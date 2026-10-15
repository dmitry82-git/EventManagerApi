using EventManagerApi.Models;
using EventManagerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(EventService eventService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Event>>> GetAll() =>
        Ok(await eventService.GetAllAsync());
}