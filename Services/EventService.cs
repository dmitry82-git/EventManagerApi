using EventManagerApi.Models;

namespace EventManagerApi.Services;

public class EventService
{
    private static readonly List<Event> Events =
    [
        new Event(1, "Sunrise Yoga in the Park",
            "Gentle 60-minute vinyasa flow for all levels. Bring your own mat.",
            new DateTime(2026, 11, 7, 7, 30, 0), new DateTime(2026, 11, 7, 8, 30, 0),
            "Riverside Park, Main Lawn", EventType.Fitness, 18, true),

        new Event(2, "City 10K Fun Run",
            "Timed 10 km run through the old town. Medals for all finishers.",
            new DateTime(2026, 11, 14, 9, 0, 0), new DateTime(2026, 11, 14, 12, 0, 0),
            "Central Square", EventType.Fitness, 120, true),

        new Event(3, "Stand-Up Comedy Night",
            "Five local comedians and a headline act. Doors open at 19:00.",
            new DateTime(2026, 11, 12, 20, 0, 0), new DateTime(2026, 11, 12, 22, 30, 0),
            "The Laughing Fox Theatre", EventType.Entertainment, 0, true),

        new Event(4, "Open-Air Cinema: Classic Films",
            "A double feature under the stars. Blankets and snacks welcome.",
            new DateTime(2026, 11, 20, 18, 30, 0), new DateTime(2026, 11, 20, 23, 0, 0),
            "Harbour Terrace", EventType.Entertainment, 85, true),

        new Event(5, "Intro to Pottery Workshop",
            "Hands-on beginner session. You will make and glaze two pieces. Materials included.",
            new DateTime(2026, 11, 15, 14, 0, 0), new DateTime(2026, 11, 15, 17, 0, 0),
            "Clay & Co Studio, 12 Mill Street", EventType.Workshop, 8, true),

        new Event(6, "Product Design Careers Meetup",
            "Lightning talks from three designers followed by open networking.",
            new DateTime(2026, 11, 18, 18, 0, 0), new DateTime(2026, 11, 18, 21, 0, 0),
            "Hub Coworking, 3rd Floor", EventType.Networking, 40, true),

        new Event(7, "Jazz at the Old Mill",
            "Live quartet performing standards and originals. Food and drinks available.",
            new DateTime(2026, 11, 27, 19, 30, 0), new DateTime(2026, 11, 27, 22, 0, 0),
            "The Old Mill Café", EventType.Music, 60, true),

    ];

    public Task<IReadOnlyList<Event>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Event>>(Events.ToList());
}