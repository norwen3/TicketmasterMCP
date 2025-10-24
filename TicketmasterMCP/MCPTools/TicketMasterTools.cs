using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using ModelContextProtocol.Server;
using TicketmasterMCP.Services;

namespace TicketmasterMCP;

[McpServerToolType]
public sealed class TicketMasterTools
{
    private readonly ITicketMasterService _ticketMasterService;
    private readonly ILogger<TicketMasterTools> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public TicketMasterTools(ITicketMasterService ticketMasterService, ILogger<TicketMasterTools> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _ticketMasterService = ticketMasterService;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [McpServerTool, Description("Search for venues using keywords and location filters")]
    public async Task<string> SearchVenuesAsync(
        [Description("Search keyword for venues")] string keyword,
        [Description("City name (optional)")] string? city = null,
        [Description("State code (optional)")] string? state = null,
        [Description("Country code (optional)")] string? country = null)
    {
        try
        {
            _logger.LogInformation("Searching venues with keyword: {Keyword}, city: {City}, state: {State}, country: {Country}", 
                keyword, city, state, country);
            
            var result = await _ticketMasterService.SearchVenuesAsync(keyword, city, state, country);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching venues with keyword: {Keyword}", keyword);
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Get detailed information about a specific venue")]
    public async Task<string> GetVenueDetailsAsync(
        [Description("Venue ID to get details for")] string venueId)
    {
        try
        {
            _logger.LogInformation("Getting venue details for ID: {VenueId}", venueId);
            
            var result = await _ticketMasterService.GetVenueDetailsAsync(venueId);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue details for ID: {VenueId}", venueId);
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Search for events using keywords and location filters")]
    public async Task<string> SearchEventsAsync(
        [Description("Search keyword for events")] string keyword,
        [Description("City name (optional)")] string? city = null,
        [Description("State code (optional)")] string? state = null,
        [Description("Country code (optional)")] string? country = null,
        [Description("Start date for events (optional, format: yyyy-MM-dd)")] string? startDate = null,
        [Description("End date for events (optional, format: yyyy-MM-dd)")] string? endDate = null)
    {
        try
        {
            _logger.LogInformation("Searching events with keyword: {Keyword}, city: {City}, state: {State}, country: {Country}, startDate: {StartDate}, endDate: {EndDate}", 
                keyword, city, state, country, startDate, endDate);
            
            DateTime? startDateTime = null;
            DateTime? endDateTime = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                startDateTime = start;
            
            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                endDateTime = end;

            var result = await _ticketMasterService.SearchEventsAsync(keyword, city, state, country, startDateTime, endDateTime);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events with keyword: {Keyword}", keyword);
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Get all venues (aggregates pages)")]
    public async Task<string> GetAllVenuesAsync()
    {
        try
        {
            _logger.LogInformation("Getting all venues (paged)");
            var result = await _ticketMasterService.GetAllVenuesAsync();
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all venues");
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Get all venues using API key from appsettings.json")]
    public async Task<string> GetAllVenuesFromConfigAsync()
    {
        try
        {
            var apiKey = _configuration["TicketMaster:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                var msg = "TicketMaster API key not found in configuration (TicketMaster:ApiKey).";
                _logger.LogWarning(msg);
                return JsonSerializer.Serialize(new { error = msg });
            }

            _logger.LogInformation("Getting all venues using API key from configuration");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://app.ticketmaster.com/discovery/v2/");

            var allVenues = new List<TicketmasterMCP.Models.Venue>();
            int page = 0;
            int totalPages = int.MaxValue;

            while (page < totalPages)
            {
                var url = $"venues.json?apikey={Uri.EscapeDataString(apiKey)}&page={page}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("page", out var pageElem) && pageElem.TryGetProperty("totalPages", out var tp))
                    {
                        totalPages = tp.GetInt32();
                    }
                }
                catch
                {
                    // ignore
                }

                var root = JsonSerializer.Deserialize<TicketmasterMCP.Models.Root>(json);
                if (root?.Embedded?.Venues != null && root.Embedded.Venues.Count > 0)
                {
                    allVenues.AddRange(root.Embedded.Venues);
                }
                else
                {
                    break;
                }

                page++;
                if (page > 1000) break; // safety
            }

            var aggregated = new TicketmasterMCP.Models.Root { Embedded = new TicketmasterMCP.Models.Embedded { Venues = allVenues } };
            return JsonSerializer.Serialize(aggregated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all venues from config");
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Get detailed information about a specific event")]
    public async Task<string> GetEventDetailsAsync(
        [Description("Event ID to get details for")] string eventId)
    {
        try
        {
            _logger.LogInformation("Getting event details for ID: {EventId}", eventId);
            
            var result = await _ticketMasterService.GetEventDetailsAsync(eventId);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event details for ID: {EventId}", eventId);
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [McpServerTool, Description("Get a limited number of venues")]
    public async Task<string> GetLimitedVenuesAsync(
        [Description("Number of venues to return (default: 10)")] int limit = 10)
    {
        try
        {
            _logger.LogInformation("Getting {Limit} venues", limit);
            var result = await _ticketMasterService.GetLimitedVenuesAsync(limit);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting limited venues");
            return JsonSerializer.Serialize(new { error = ex.Message, details = ex.ToString() });
        }
    }
}
