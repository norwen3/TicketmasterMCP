using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicketmasterMCP.Models;
public class BoxOfficeInfo
{
    [JsonPropertyName("phoneNumberDetail")]
    public string PhoneNumberDetail { get; set; }

    [JsonPropertyName("openHoursDetail")]
    public string OpenHoursDetail { get; set; }

    [JsonPropertyName("acceptedPaymentDetail")]
    public string AcceptedPaymentDetail { get; set; }

    [JsonPropertyName("willCallDetail")]
    public string WillCallDetail { get; set; }
}
