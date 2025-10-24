using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Location
{
    [JsonPropertyName("longitude")]
    public string Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; }
}
