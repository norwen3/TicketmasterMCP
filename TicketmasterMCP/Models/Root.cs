using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Root
{
    [JsonPropertyName("_embedded")]
    public Embedded Embedded { get; set; }
}
