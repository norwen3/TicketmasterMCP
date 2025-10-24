using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;
public class Ada
{
    [JsonPropertyName("adaPhones")]
    public string AdaPhones { get; set; }

    [JsonPropertyName("adaCustomCopy")]
    public string AdaCustomCopy { get; set; }

    [JsonPropertyName("adaHours")]
    public string AdaHours { get; set; }
}