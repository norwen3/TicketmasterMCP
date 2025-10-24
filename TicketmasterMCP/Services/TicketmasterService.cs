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
            var response = await _httpClient.GetAsync($"venues?{queryString}");
            
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
                var response = await _httpClient.GetAsync($"venues?{queryString}");
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
            var response = await _httpClient.GetAsync($"venues/{venueId}?apikey={_apiKey}");
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
            var response = await _httpClient.GetAsync($"venues?{queryString}");
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
                $"keyword={Uri.EscapeDataString(keyword)}",
                "locale=*"
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
            var response = await _httpClient.GetAsync($"events?{queryString}");
            
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
            var response = await _httpClient.GetAsync($"events/{eventId}?apikey={_apiKey}");
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

    public async Task<Root> SearchLimitedVenuesByCityAsync(string city, string? country = null, int limit = 10)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"apikey={_apiKey}",
                $"city={Uri.EscapeDataString(city)}",
                $"size={limit}",
                "page=0"
            };

            if (!string.IsNullOrEmpty(country))
                queryParams.Add($"countryCode={Uri.EscapeDataString(country)}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"venues?{queryString}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Root>(json) ?? new Root();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching limited venues for city: {City}", city);
            throw;
        }
    }

    public async Task<Root> GetAllVenuesByCityAsync(string city, string? country = null)
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
                    $"city={Uri.EscapeDataString(city)}",
                    $"page={page}",
                    "size=200" // Maximum page size for efficiency
                };

                if (!string.IsNullOrEmpty(country))
                    queryParams.Add($"countryCode={Uri.EscapeDataString(country)}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"venues?{queryString}");
                
                // Handle rate limiting
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Rate limit hit, waiting 3 seconds before retrying page {Page}", page);
                    await Task.Delay(3000); // Wait 3 seconds
                    continue; // Retry the same page
                }
                
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                // Try to parse page.totalPages if present
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("page", out var pageElem))
                    {
                        if (pageElem.TryGetProperty("totalPages", out var tp))
                        {
                            totalPages = tp.GetInt32();
                        }
                        
                        // Also log current page info for debugging
                        if (pageElem.TryGetProperty("number", out var currentPageElem) && 
                            pageElem.TryGetProperty("size", out var sizeElem) &&
                            pageElem.TryGetProperty("totalElements", out var totalElem))
                        {
                            _logger.LogInformation("Page {CurrentPage} of {TotalPages}, Size: {Size}, Total Elements: {Total}",
                                currentPageElem.GetInt32(), totalPages, sizeElem.GetInt32(), totalElem.GetInt32());
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not parse pagination info from response");
                }

                var root = JsonSerializer.Deserialize<Root>(json);
                if (root?.Embedded?.Venues != null && root.Embedded.Venues.Count > 0)
                {
                    allVenues.AddRange(root.Embedded.Venues);
                    _logger.LogInformation("Found {Count} venues on page {Page}, total so far: {Total}", 
                        root.Embedded.Venues.Count, page, allVenues.Count);
                }
                else
                {
                    _logger.LogInformation("No more venues found on page {Page}, stopping", page);
                    break; // no more venues
                }

                page++;

                // Add delay between requests to avoid rate limiting
                if (page < totalPages)
                {
                    await Task.Delay(1000); // Wait 1 second between requests
                }

                // Safety limit to prevent infinite loops
                if (page > 100)
                {
                    _logger.LogWarning("Reached safety limit of 100 pages, stopping venue retrieval");
                    break;
                }
            }

            _logger.LogInformation("Retrieved total of {Count} venues for city: {City}", allVenues.Count, city);
            return new Root { Embedded = new Embedded { Venues = allVenues } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all venues for city: {City}", city);
            throw;
        }
    }
}
