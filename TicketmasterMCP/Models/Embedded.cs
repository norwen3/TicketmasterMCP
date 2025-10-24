using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Embedded
{
    [JsonPropertyName("venues")]
    public List<Venue> Venues { get; set; }
}

