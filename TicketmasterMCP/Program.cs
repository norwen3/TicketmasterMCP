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


// Get next 5 upcoming events for a user-specified city ordered by date
using (var scope = host.Services.CreateScope())
{
    var svc = scope.ServiceProvider.GetRequiredService<ITicketMasterService>();
    
    Console.Write("Enter city name to search for events: ");
    var cityName = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(cityName))
    {
        Console.WriteLine("No city name provided. Exiting.");
        return;
    }
    
    Console.WriteLine($"Searching for the next 5 upcoming events in {cityName} ordered by date...\n");
    
    try
    {
        // Search for events in the specified city
        Console.WriteLine($"Searching for events in {cityName}...");
        var result = await svc.SearchEventsAsync("", cityName, "", "NO", null, null);
        
        var events = result?.Embedded?.Events;
        
        if (events != null && events.Count > 0)
        {
            // Filter events that are in the specified city
            var cityEvents = events.Where(e => 
                e.Embedded?.Venues?.Any(v => 
                    v.City?.Name?.Contains(cityName, StringComparison.OrdinalIgnoreCase) == true
                ) == true
            ).ToList();
            
            if (cityEvents.Count == 0)
            {
                Console.WriteLine($"No events found specifically in {cityName}. Showing all events from search:");
                cityEvents = events.ToList();
            }
            
            // Sort events by date (upcoming first) and take only next 5
            var now = DateTime.Now;
            var upcomingEvents = cityEvents
                .Where(e => DateTime.TryParse(e.Dates?.Start?.DateTime, out var eventDate) && eventDate > now)
                .OrderBy(e => DateTime.Parse(e.Dates?.Start?.DateTime ?? DateTime.MaxValue.ToString()))
                .Take(5)
                .ToList();
            
            Console.WriteLine($"Next {upcomingEvents.Count} upcoming events in {cityName} (ordered by date):");
            Console.WriteLine(new string('=', 80));
            
            foreach (var eventItem in upcomingEvents)
            {
                Console.WriteLine($"EVENT: {eventItem.Name}");
                Console.WriteLine($"ID: {eventItem.Id}");
                
                // Event date and time
                if (eventItem.Dates?.Start != null)
                {
                    if (!string.IsNullOrEmpty(eventItem.Dates.Start.LocalDate))
                        Console.WriteLine($"Date: {eventItem.Dates.Start.LocalDate}");
                    if (!string.IsNullOrEmpty(eventItem.Dates.Start.LocalTime))
                        Console.WriteLine($"Time: {eventItem.Dates.Start.LocalTime}");
                }
                
                // Venue information
                var venue = eventItem.Embedded?.Venues?.FirstOrDefault();
                if (venue != null)
                {
                    Console.WriteLine($"Venue: {venue.Name}");
                    if (!string.IsNullOrEmpty(venue.Address?.Line1))
                        Console.WriteLine($"Address: {venue.Address.Line1}");
                    if (!string.IsNullOrEmpty(venue.City?.Name))
                        Console.WriteLine($"City: {venue.City.Name}");
                }
                
                // Event classification (genre, type, etc.)
                if (eventItem.Classifications != null && eventItem.Classifications.Count > 0)
                {
                    var classification = eventItem.Classifications.First();
                    if (classification.Genre?.Name != null)
                        Console.WriteLine($"Genre: {classification.Genre.Name}");
                    if (classification.Segment?.Name != null)
                        Console.WriteLine($"Category: {classification.Segment.Name}");
                }
                
                // Price range
                if (eventItem.PriceRanges != null && eventItem.PriceRanges.Count > 0)
                {
                    var priceRange = eventItem.PriceRanges.First();
                    Console.WriteLine($"Price Range: {priceRange.Min} - {priceRange.Max} {priceRange.Currency}");
                }
                
                // Ticketmaster URL
                if (!string.IsNullOrEmpty(eventItem.Url))
                    Console.WriteLine($"Ticketmaster URL: {eventItem.Url}");
                
                Console.WriteLine(new string('-', 80));
            }
        }
        else
        {
            Console.WriteLine($"No events found in {cityName}.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error searching for events in {cityName}: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    Console.WriteLine("\nPress any key to exit...");
    try
    {
        Console.ReadKey();
    }
    catch (InvalidOperationException)
    {
        // Handle the case when console input is redirected
        Console.Read();
    }
}

/*
// Original Interactive menu - commented out for Oslo search
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
*/