using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;
public class UpcomingEvents
{
    [JsonPropertyName("ticketmaster")]
    public int Ticketmaster { get; set; }

    [JsonPropertyName("_total")]
    public int Total { get; set; }

    [JsonPropertyName("_filtered")]
    public int Filtered { get; set; }
}
