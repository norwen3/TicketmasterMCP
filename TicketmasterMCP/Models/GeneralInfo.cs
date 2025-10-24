using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;
public class GeneralInfo
{
    [JsonPropertyName("generalRule")]
    public string GeneralRule { get; set; }

    [JsonPropertyName("childRule")]
    public string ChildRule { get; set; }
}