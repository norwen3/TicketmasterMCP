using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TicketmasterMCP.Models;

namespace TicketmasterMCP.Services;

public class TicketmasterService
{
    private readonly HttpClient _httpClient;

    public TicketmasterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    List<Root> roots = new();

    public async Task<List<Root>> GetVenuesAsync(string apiKey, int pages = 1)
    {
        if(roots?.Count > 0) 
            return roots;

        var response = await _httpClient.GetAsync($"https://app.ticketmaster.com/discovery/v2/venues.json?apikey={apiKey}");
        if(response.IsSuccessStatusCode)
        {
            roots = await response.Content.ReadFromJsonAsync(RootContext.Default.ListRoot) ?? [];
        }

        roots ??= [];

        return roots;
    }

    public async Task<Root> GetVenueAsync(string apiKey, string name)
    {
        var roots = await GetVenuesAsync(apiKey);
        return roots.FirstOrDefault(r => r.Embedded.Venues.Any(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }
}
