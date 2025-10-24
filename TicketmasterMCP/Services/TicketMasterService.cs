using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TicketmasterMCP.Models;

namespace TicketmasterMCP.Services;

public class TicketMasterService : ITicketMasterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TicketMasterService> _logger;
    private readonly string _apiKey;

    public TicketMasterService(HttpClient httpClient, ILogger<TicketMasterService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration?["TicketMaster:ApiKey"] ?? Environment.GetEnvironmentVariable("TICKETMASTER_API_KEY") ?? "arPcZKZ38ZEHN4oBab9gGIEv8NP7f1GU";
        
        _httpClient.BaseAddress = new Uri("https://app.ticketmaster.com/discovery/v2/");
    }

    public async Task<Root> SearchVenuesAsync(string keyword, string? city = null, string? state = null, string? country = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"apikey={_apiKey}",
                $"keyword={Uri.EscapeDataString(keyword)}"
            };

            if (!string.IsNullOrEmpty(city))
                queryParams.Add($"city={Uri.EscapeDataString(city)}");
            if (!string.IsNullOrEmpty(state))
                queryParams.Add($"stateCode={Uri.EscapeDataString(state)}");
            if (!string.IsNullOrEmpty(country))
                queryParams.Add($"countryCode={Uri.EscapeDataString(country)}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}?{queryString}");
            
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching venues for keyword: {Keyword}", keyword);
            throw;
        }
    }

    public async Task<Root> GetAllVenuesAsync()
    {
        try
        {
            var allVenues = new List<Venue>();
            int page = 0;
            int totalPages = int.MaxValue;

            while (page < totalPages)
            {
                var queryParams = new List<string>
                {
                    $"apikey={_apiKey}",
                    $"page={page}"
                };

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"venues.json?{queryString}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                // Try to parse page.totalPages if present
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
                    // ignore parsing errors, will fall back to breaking on empty results
                }

                var root = JsonSerializer.Deserialize<Root>(json);
                if (root?.Embedded?.Venues != null && root.Embedded.Venues.Count > 0)
                {
                    allVenues.AddRange(root.Embedded.Venues);
                }
                else
                {
                    break; // no more venues
                }

                page++;

                // safety limit
                if (page > 1000) break;
            }

            return new Root { Embedded = new Embedded { Venues = allVenues } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all venues");
            throw;
        }
    }

    public async Task<Root> GetVenueDetailsAsync(string venueId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"venues/{venueId}.json?apikey={_apiKey}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue details for ID: {VenueId}", venueId);
            throw;
        }
    }

    public async Task<Root> GetLimitedVenuesAsync(int limit = 10)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"apikey={_apiKey}",
                $"size={limit}",
                "page=0"
            };

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"venues.json?{queryString}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting limited venues");
            throw;
        }
    }

    public async Task<Root> SearchEventsAsync(string keyword, string? city = null, string? state = null, string? country = null, DateTime? startDateTime = null, DateTime? endDateTime = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"apikey={_apiKey}",
                $"keyword={Uri.EscapeDataString(keyword)}"
            };

            if (!string.IsNullOrEmpty(city))
                queryParams.Add($"city={Uri.EscapeDataString(city)}");
            if (!string.IsNullOrEmpty(state))
                queryParams.Add($"stateCode={Uri.EscapeDataString(state)}");
            if (!string.IsNullOrEmpty(country))
                queryParams.Add($"countryCode={Uri.EscapeDataString(country)}");
            if (startDateTime.HasValue)
                queryParams.Add($"startDateTime={startDateTime.Value:yyyy-MM-ddTHH:mm:ssZ}");
            if (endDateTime.HasValue)
                queryParams.Add($"endDateTime={endDateTime.Value:yyyy-MM-ddTHH:mm:ssZ}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"events.json?{queryString}");
            
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events for keyword: {Keyword}", keyword);
            throw;
        }
    }

    public async Task<Root> GetEventDetailsAsync(string eventId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"events/{eventId}.json?apikey={_apiKey}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event details for ID: {EventId}", eventId);
            throw;
        }
    }
}
