using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;

public class Address
{
    [JsonPropertyName("line1")]
    public string Line1 { get; set; }
}