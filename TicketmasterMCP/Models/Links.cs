using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Links
{
    [JsonPropertyName("self")]
    public Self Self { get; set; }
}
