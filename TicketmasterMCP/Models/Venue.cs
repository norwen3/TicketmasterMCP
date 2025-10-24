using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Venue
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("test")]
    public bool Test { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }

    [JsonPropertyName("images")]
    public List<Image> Images { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("city")]
    public City City { get; set; }

    [JsonPropertyName("country")]
    public Country Country { get; set; }

    [JsonPropertyName("address")]
    public Address Address { get; set; }

    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonPropertyName("markets")]
    public List<Market> Markets { get; set; }

    [JsonPropertyName("dmas")]
    public List<Dma> Dmas { get; set; }

    [JsonPropertyName("boxOfficeInfo")]
    public BoxOfficeInfo BoxOfficeInfo { get; set; }

    [JsonPropertyName("parkingDetail")]
    public string ParkingDetail { get; set; }

    [JsonPropertyName("accessibleSeatingDetail")]
    public string AccessibleSeatingDetail { get; set; }

    [JsonPropertyName("generalInfo")]
    public GeneralInfo GeneralInfo { get; set; }

    [JsonPropertyName("upcomingEvents")]
    public UpcomingEvents UpcomingEvents { get; set; }

    [JsonPropertyName("ada")]
    public Ada Ada { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}
