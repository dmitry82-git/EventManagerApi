using EventManagerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]   // all URLs in this class start with /api/events
public class EventsController : ControllerBase
{
    // The database is a list kept in memory. It is "static" so there is only
    // one list shared by all requests (ASP.NET creates a new controller for
    // every request, so a normal field would be emptied each time).
    // The list resets whenever the app restarts.
    private static readonly List<Event> Events = new List<Event>
    {
        new Event
        {
            Id = 1, Title = "Sunrise Yoga in the Park",
            Description = "Gentle 60-minute flow for all levels.",
            DateStartTime = new DateTime(2026, 11, 7, 7, 30, 0),
            DateEndTime = new DateTime(2026, 11, 7, 8, 30, 0),
            Location = "Riverside Park", Type = EventType.Fitness,
            PlacesAvailable = 18, Status = EventStatus.Published,
            AttendeeEmails = new List<string> { "anna@example.com", "ben@example.com" }
        },
        new Event
        {
            Id = 2, Title = "Jazz at the Old Mill",
            Description = "Live quartet playing standards and originals.",
            DateStartTime = new DateTime(2026, 11, 27, 19, 30, 0),
            DateEndTime = new DateTime(2026, 11, 27, 22, 0, 0),
            Location = "The Old Mill Cafe", Type = EventType.Music,
            PlacesAvailable = 60, Status = EventStatus.Published,
            AttendeeEmails = new List<string> { "carla@example.com" }
        },
        new Event
        {
            Id = 3, Title = "Intro to Pottery Workshop",
            Description = "Hands-on beginner session.",
            DateStartTime = new DateTime(2026, 11, 15, 14, 0, 0),
            DateEndTime = new DateTime(2026, 11, 15, 17, 0, 0),
            Location = "Clay & Co Studio", Type = EventType.Workshop,
            PlacesAvailable = 8   // no Status given, so it is a Draft
        },
        new Event
        {
            Id = 4, Title = "Winter Choir Concert",
            Description = "Annual community choir performance.",
            DateStartTime = new DateTime(2026, 12, 12, 17, 0, 0),
            DateEndTime = new DateTime(2026, 12, 12, 19, 0, 0),
            Location = "St. Mary's Church", Type = EventType.Music,
            PlacesAvailable = 150
        }
    };

    // The id to give to the next event we create.
    private static int nextId = 5;

    // ---------- VIEW A LIST OF EVENTS ----------
    // GET /api/events               -> every event
    // GET /api/events?status=Draft  -> only drafts
    [HttpGet]
    public ActionResult<List<Event>> GetAll([FromQuery] EventStatus? status)
    {
        // "EventStatus?" means the status is optional (it can be null).
        // Where(...) keeps only the events that match the condition.
        // OrderBy(...) sorts them with the soonest event first.
        var result = Events
            .Where(e => status == null || e.Status == status)
            .OrderBy(e => e.DateStartTime)
            .ToList();

        return Ok(result);   // 200 OK + the list as JSON
    }

    // GET /api/events/3  -> one event
    [HttpGet("{id:int}")]
    public ActionResult<Event> GetById(int id)
    {
        var evt = FindEvent(id);
        if (evt == null) return NotFound($"Event {id} was not found.");   // 404

        return Ok(evt);
    }

    // ---------- CREATE A DRAFT EVENT ----------
    // POST /api/events   (event data is sent in the JSON body)
    [HttpPost]
    public ActionResult<Event> CreateDraft(Event newEvent)
    {
        var error = CheckEvent(newEvent);
        if (error != null) return BadRequest(error);   // 400

        // Never trust the client for these fields: we set them ourselves.
        newEvent.Id = nextId;
        nextId++;
        newEvent.Status = EventStatus.Draft;
        newEvent.AttendeeEmails = new List<string>();

        Events.Add(newEvent);

        // 201 Created, plus a link to where the new event can be fetched.
        return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, newEvent);
    }

    // ---------- UPDATE A DRAFT OR A PUBLISHED EVENT ----------
    // PUT /api/events/3
    [HttpPut("{id:int}")]
    public ActionResult<Event> Update(int id, Event updated)
    {
        var evt = FindEvent(id);
        if (evt == null) return NotFound($"Event {id} was not found.");

        if (evt.Status == EventStatus.Cancelled)
            return Conflict("A cancelled event cannot be updated.");   // 409

        var error = CheckEvent(updated);
        if (error != null) return BadRequest(error);

        // Copy over the editable fields only.
        // Id, Status and AttendeeEmails stay as they are.
        evt.Title = updated.Title;
        evt.Description = updated.Description;
        evt.DateStartTime = updated.DateStartTime;
        evt.DateEndTime = updated.DateEndTime;
        evt.Location = updated.Location;
        evt.Type = updated.Type;
        evt.PlacesAvailable = updated.PlacesAvailable;

        // Only a published event has registered attendees to tell.
        if (evt.Status == EventStatus.Published)
            NotifyAttendees(evt);

        return Ok(evt);
    }

    // ---------- PUBLISH A DRAFT ----------
    // POST /api/events/3/publish
    [HttpPost("{id:int}/publish")]
    public ActionResult<Event> Publish(int id)
    {
        var evt = FindEvent(id);
        if (evt == null) return NotFound($"Event {id} was not found.");

        if (evt.Status != EventStatus.Draft)
            return Conflict($"Only drafts can be published. This event is {evt.Status}.");

        evt.Status = EventStatus.Published;
        return Ok(evt);
    }

    // ---------- DELETE A DRAFT ----------
    // DELETE /api/events/3
    [HttpDelete("{id:int}")]
    public IActionResult DeleteDraft(int id)
    {
        var evt = FindEvent(id);
        if (evt == null) return NotFound($"Event {id} was not found.");

        if (evt.Status != EventStatus.Draft)
            return Conflict($"Only drafts can be deleted. This event is {evt.Status}.");

        Events.Remove(evt);
        return NoContent();   // 204: success, nothing to send back
    }

    // ---------- CANCEL A PUBLISHED EVENT ----------
    // POST /api/events/3/cancel
    [HttpPost("{id:int}/cancel")]
    public ActionResult<Event> Cancel(int id)
    {
        var evt = FindEvent(id);
        if (evt == null) return NotFound($"Event {id} was not found.");

        if (evt.Status != EventStatus.Published)
            return Conflict($"Only published events can be cancelled. This event is {evt.Status}.");

        evt.Status = EventStatus.Cancelled;
        return Ok(evt);
    }

    // ---------- HELPER METHODS (private = only used inside this class) ----------

    // Looks for an event with this id. Returns null if there is none.
    private static Event? FindEvent(int id)
    {
        return Events.FirstOrDefault(e => e.Id == id);
    }

    // Checks the data a client sent. Returns an error message,
    // or null when everything is fine.
    private static string? CheckEvent(Event e)
    {
        if (string.IsNullOrWhiteSpace(e.Title))
            return "Title is required.";

        if (string.IsNullOrWhiteSpace(e.Location))
            return "Location is required.";

        if (e.DateEndTime <= e.DateStartTime)
            return "The end time must be after the start time.";

        if (e.PlacesAvailable < 0)
            return "Places available cannot be negative.";

        return null;
    }

    // Simulate sending emails by printing to the console window.
    private static void NotifyAttendees(Event evt)
    {
        foreach (var email in evt.AttendeeEmails)
        {
            Console.WriteLine($"Notification to {email}: the event '{evt.Title}' has been updated.");
        }
    }
}