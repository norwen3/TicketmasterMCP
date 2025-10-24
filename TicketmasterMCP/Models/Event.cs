using System.Text.Json.Serialization;

namespace TicketmasterMCP.Models;

public class Event
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("test")]
    public bool? Test { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }

    [JsonPropertyName("images")]
    public List<Image> Images { get; set; }

    [JsonPropertyName("sales")]
    public Sales Sales { get; set; }

    [JsonPropertyName("dates")]
    public EventDates Dates { get; set; }

    [JsonPropertyName("classifications")]
    public List<Classification> Classifications { get; set; }

    [JsonPropertyName("promoter")]
    public Promoter Promoter { get; set; }

    [JsonPropertyName("info")]
    public string Info { get; set; }

    [JsonPropertyName("pleaseNote")]
    public string PleaseNote { get; set; }

    [JsonPropertyName("priceRanges")]
    public List<PriceRange> PriceRanges { get; set; }

    [JsonPropertyName("products")]
    public List<Product> Products { get; set; }

    [JsonPropertyName("seatmap")]
    public Seatmap Seatmap { get; set; }

    [JsonPropertyName("accessibility")]
    public Accessibility Accessibility { get; set; }

    [JsonPropertyName("ticketLimit")]
    public TicketLimit TicketLimit { get; set; }

    [JsonPropertyName("ageRestrictions")]
    public AgeRestrictions AgeRestrictions { get; set; }

    [JsonPropertyName("_embedded")]
    public EventEmbedded Embedded { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}

public class Sales
{
    [JsonPropertyName("public")]
    public SaleInfo Public { get; set; }
}

public class SaleInfo
{
    [JsonPropertyName("startDateTime")]
    public string StartDateTime { get; set; }

    [JsonPropertyName("startTBD")]
    public bool? StartTBD { get; set; }

    [JsonPropertyName("endDateTime")]
    public string EndDateTime { get; set; }
}

public class EventDates
{
    [JsonPropertyName("start")]
    public DateStart Start { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("status")]
    public DateStatus Status { get; set; }

    [JsonPropertyName("spanMultipleDays")]
    public bool? SpanMultipleDays { get; set; }
}

public class DateStart
{
    [JsonPropertyName("localDate")]
    public string LocalDate { get; set; }

    [JsonPropertyName("localTime")]
    public string LocalTime { get; set; }

    [JsonPropertyName("dateTime")]
    public string DateTime { get; set; }

    [JsonPropertyName("dateTBD")]
    public bool? DateTBD { get; set; }

    [JsonPropertyName("dateTBA")]
    public bool? DateTBA { get; set; }

    [JsonPropertyName("timeTBA")]
    public bool? TimeTBA { get; set; }

    [JsonPropertyName("noSpecificTime")]
    public bool? NoSpecificTime { get; set; }
}

public class DateStatus
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
}

public class Classification
{
    [JsonPropertyName("primary")]
    public bool? Primary { get; set; }

    [JsonPropertyName("segment")]
    public ClassificationItem Segment { get; set; }

    [JsonPropertyName("genre")]
    public ClassificationItem Genre { get; set; }

    [JsonPropertyName("subGenre")]
    public ClassificationItem SubGenre { get; set; }

    [JsonPropertyName("type")]
    public ClassificationItem Type { get; set; }

    [JsonPropertyName("subType")]
    public ClassificationItem SubType { get; set; }

    [JsonPropertyName("family")]
    public bool? Family { get; set; }
}

public class ClassificationItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Promoter
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class PriceRange
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("min")]
    public decimal? Min { get; set; }

    [JsonPropertyName("max")]
    public decimal? Max { get; set; }
}

public class Product
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("classifications")]
    public List<Classification> Classifications { get; set; }
}

public class Seatmap
{
    [JsonPropertyName("staticUrl")]
    public string StaticUrl { get; set; }
}

public class Accessibility
{
    [JsonPropertyName("ticketLimit")]
    public int? TicketLimit { get; set; }
}

public class TicketLimit
{
    [JsonPropertyName("info")]
    public string Info { get; set; }
}

public class AgeRestrictions
{
    [JsonPropertyName("legalAgeEnforced")]
    public bool? LegalAgeEnforced { get; set; }
}

public class EventEmbedded
{
    [JsonPropertyName("venues")]
    public List<Venue> Venues { get; set; }

    [JsonPropertyName("attractions")]
    public List<Attraction> Attractions { get; set; }
}

public class Attraction
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("test")]
    public bool? Test { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }

    [JsonPropertyName("images")]
    public List<Image> Images { get; set; }

    [JsonPropertyName("classifications")]
    public List<Classification> Classifications { get; set; }

    [JsonPropertyName("upcomingEvents")]
    public UpcomingEvents UpcomingEvents { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}