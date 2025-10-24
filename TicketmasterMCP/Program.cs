using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TicketmasterMCP;
using TicketmasterMCP.Services;
using TicketmasterMCP.Models;
using System.Text.Json;
using System.IO;
using System.Linq;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Add logging
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Register HTTP client for TicketMaster API
builder.Services.AddHttpClient<TicketMasterService>();

// Register TicketMaster service
builder.Services.AddScoped<ITicketMasterService, TicketMasterService>();

// Register MCP server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<TicketMasterTools>();

var host = builder.Build();


// Removed all references to limited_venues.json. The app now always fetches venues from the Ticketmaster API.


// Interactive menu
using (var scope = host.Services.CreateScope())
{
    var svc = scope.ServiceProvider.GetRequiredService<ITicketMasterService>();
    while (true)
    {
        Console.WriteLine("\nTicketmasterMCP Menu:");
        Console.WriteLine("1) List 10 venues");
        Console.WriteLine("2) Get details for a specific venue by name");
        Console.WriteLine("3) Get a random venue");
        Console.WriteLine("4) Get all venues for a city");
        Console.WriteLine("5) Exit");
        Console.Write("Select an option: ");
        var input = Console.ReadLine();
        if (input == "1")
        {
            var result = await svc.GetLimitedVenuesAsync(10);
            var venues = result?.Embedded?.Venues;
            if (venues != null && venues.Count > 0)
            {
                Console.WriteLine("\nFirst 10 venues:");
                foreach (var v in venues)
                {
                    Console.WriteLine($"- {v.Name} ({v.City?.Name}, {v.Country?.CountryCode})");
                }
            }
            else
            {
                Console.WriteLine("No venues found.");
            }
        }
        else if (input == "2")
        {
            Console.Write("Enter venue name: ");
            var name = Console.ReadLine();
            var result = await svc.GetLimitedVenuesAsync(100); // Search first 100 venues for demo
            var venue = result?.Embedded?.Venues?.FirstOrDefault(v => v.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
            if (venue != null)
            {
                var details = await svc.GetVenueDetailsAsync(venue.Id);
                Console.WriteLine(JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("Venue not found in the first 100 venues.");
            }
        }
        else if (input == "3")
        {
            var result = await svc.GetLimitedVenuesAsync(100);
            var venues = result?.Embedded?.Venues;
            if (venues != null && venues.Count > 0)
            {
                var rand = new Random();
                var venue = venues[rand.Next(venues.Count)];
                Console.WriteLine($"Random venue: {venue.Name} ({venue.City?.Name}, {venue.Country?.CountryCode})");
                var details = await svc.GetVenueDetailsAsync(venue.Id);
                Console.WriteLine(JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("No venues found.");
            }
        }
        else if (input == "4")
        {
            Console.Write("Enter city name: ");
            var city = Console.ReadLine();
            var result = await svc.SearchVenuesAsync("", city);
            var venues = result?.Embedded?.Venues;
            if (venues != null && venues.Count > 0)
            {
                Console.WriteLine($"\nVenues in {city}:");
                foreach (var v in venues)
                {
                    Console.WriteLine($"- {v.Name} ({v.Address?.Line1})");
                }
            }
            else
            {
                Console.WriteLine($"No venues found for city '{city}'.");
            }
        }
        else if (input == "5")
        {
            Console.WriteLine("Exiting.");
            break;
        }
        else
        {
            Console.WriteLine("Invalid option. Please try again.");
        }
    }
}