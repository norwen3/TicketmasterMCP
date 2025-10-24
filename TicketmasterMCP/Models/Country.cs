using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;
public class Country
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; }
}