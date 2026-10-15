# EventManagerApi

The backend for **Events Manager**, a mobile app that helps event organisers plan and manage
their events. It is an ASP.NET Core Web API built to work with a React Native (Expo) app.

## What organisers can do

- Create a draft event
- Edit a draft event
- Publish a draft so it is open to attendees
- Delete a draft they no longer need
- Edit a published event, with registered attendees notified of the change
- Cancel a published event
- View all events, with each one marked as a draft, published or cancelled

## Getting started

You will need the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

The console shows the address the API is running on, for example `http://localhost:5000`.
Try it out by opening that address with `/api/events` on the end in your browser, or run:

```bash
curl http://localhost:5000/api/events
```

## Endpoints

All endpoints start with `/api/events`. Send and receive data as JSON.

### View events

**`GET /api/events`**
Returns every event, soonest first. Each event includes its status, so you can tell drafts
from published events. To see only events with a certain status, add a filter, for example
`GET /api/events?status=Draft`.

**`GET /api/events/{id}`**
Returns a single event. Responds with `404` if no event has that id.

### Create and edit events

**`POST /api/events`**
Creates a new event. Send the event details as JSON. The new event is always saved as a
draft, whatever you send for the status. Responds with `201` and the new event.

**`PUT /api/events/{id}`**
Updates an event's details. This works for drafts and published events.
When a published event is updated, its registered attendees are notified.
Cancelled events cannot be updated.

### Change an event's status

**`POST /api/events/{id}/publish`**
Publishes a draft event. Only drafts can be published.

**`POST /api/events/{id}/cancel`**
Cancels a published event. The event stays in the list, marked as cancelled.
Only published events can be cancelled.

**`DELETE /api/events/{id}`**
Permanently deletes a draft event. Published events cannot be deleted. Cancel them instead.

### Responses

- `200` The request worked
- `201` The event was created
- `204` The event was deleted
- `400` Some of the event details are missing or invalid
- `404` There is no event with that id
- `409` The event's current status does not allow this action, for example publishing an
  event that is already published

## The event

```json
{
  "id": 1,
  "title": "Sunrise Yoga in the Park",
  "description": "Gentle 60-minute flow for all levels.",
  "dateStartTime": "2026-11-07T07:30:00",
  "dateEndTime": "2026-11-07T08:30:00",
  "location": "Riverside Park",
  "type": "Fitness",
  "placesAvailable": 18,
  "status": "Published"
}
```

- **type** is one of `Fitness`, `Entertainment`, `Workshop`, `Networking` or `Music`.
- **status** starts as `Draft`, can become `Published`, and a published event can become
  `Cancelled`.
- A title and location are required, and the end time must be after the start time.

## Good to know

- Events are stored in memory, so they reset whenever the app restarts.
- Attendee notifications are simulated by a message in the console. Real emails or push
  notifications are not part of this project yet.
- There is no endpoint yet for registering attendees, so the sample events come with a few
  attendees already added.
