# TicketMaster MCP Server

This is a Model Context Protocol (MCP) server that provides access to TicketMaster's Discovery API for searching venues and events.

## Setup

1. Get a TicketMaster API key from [TicketMaster Developer Portal](https://developer.ticketmaster.com/)
2. Update the `appsettings.json` file with your API key:
   ```json
   {
     "TicketMaster": {
       "ApiKey": "YOUR_ACTUAL_API_KEY_HERE"
     }
   }
   ```

## Available Tools

### 1. Search Venues (`search_venues`)
Search for venues using keywords and location filters.

**Parameters:**
- `keyword` (required): Search keyword for venues
- `city` (optional): City name
- `state` (optional): State code
- `country` (optional): Country code

**Example:**
```json
{
  "keyword": "Madison Square Garden",
  "city": "New York",
  "state": "NY"
}
```

### 2. Get Venue Details (`get_venue_details`)
Get detailed information about a specific venue.

**Parameters:**
- `venueId` (required): Venue ID to get details for

**Example:**
```json
{
  "venueId": "KovZpZAEdFtJ"
}
```

### 3. Search Events (`search_events`)
Search for events using keywords and location filters.

**Parameters:**
- `keyword` (required): Search keyword for events
- `city` (optional): City name
- `state` (optional): State code
- `country` (optional): Country code
- `startDate` (optional): Start date for events (format: yyyy-MM-dd)
- `endDate` (optional): End date for events (format: yyyy-MM-dd)

**Example:**
```json
{
  "keyword": "concert",
  "city": "Los Angeles",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31"
}
```

### 4. Get Event Details (`get_event_details`)
Get detailed information about a specific event.

**Parameters:**
- `eventId` (required): Event ID to get details for

**Example:**
```json
{
  "eventId": "G5vYZaGqZqZqZqZ"
}
```

## Running the Server

```bash
dotnet run
```

The server will start and listen for MCP protocol messages via stdio.
