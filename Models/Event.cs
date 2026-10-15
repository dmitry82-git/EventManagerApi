using System.Text.Json.Serialization;

namespace EventManagerApi.Models;

// the annotation tells the JSON serializer to write the enum as its name instead of its number
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType
{
    Fitness,
    Entertainment,
    Workshop,
    Networking,
    Music
}

public record Event(
    int Id,
    string Title,
    string Description,
    DateTime DateStartTime,
    DateTime DateEndTime,
    string Location,
    EventType Type,
    int PlacesAvailable,
    bool IsPublished
);