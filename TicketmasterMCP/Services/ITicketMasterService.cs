using TicketmasterMCP.Models;

namespace TicketmasterMCP.Services;

public interface ITicketMasterService
{
    Task<Root> SearchVenuesAsync(string keyword, string? city = null, string? state = null, string? country = null);
    Task<Root> GetAllVenuesAsync();
    Task<Root> GetVenueDetailsAsync(string venueId);
    Task<Root> SearchEventsAsync(string keyword, string? city = null, string? state = null, string? country = null, DateTime? startDateTime = null, DateTime? endDateTime = null);
    Task<Root> GetEventDetailsAsync(string eventId);
    Task<Root> GetLimitedVenuesAsync(int limit = 10);
    Task<Root> SearchLimitedVenuesByCityAsync(string city, string? country = null, int limit = 10);
    Task<Root> GetAllVenuesByCityAsync(string city, string? country = null);
}