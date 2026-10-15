using System.Text.Json.Serialization;

namespace EventManagerApi.Models;

// The [JsonConverter] annotation makes the API send "Fitness" instead of the number 0 and so on.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType
{
    Fitness,
    Entertainment,
    Workshop,
    Networking,
    Music
}

// The three states of an event's object.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventStatus
{
    Draft,
    Published,
    Cancelled
}

// Each property below becomes a field in the JSON.
public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime DateStartTime { get; set; }
    public DateTime DateEndTime { get; set; }
    public string Location { get; set; } = "";
    public EventType Type { get; set; }
    public int PlacesAvailable { get; set; }

    // Every new event starts as a draft.
    public EventStatus Status { get; set; } = EventStatus.Draft;

    // Emails of people registered for the event.
    // [JsonIgnore] hides this list from the API output, so nobody can read
    // other people's email addresses.
    [JsonIgnore]
    public List<string> AttendeeEmails { get; set; } = new List<string>();
}