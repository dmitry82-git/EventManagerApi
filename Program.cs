using EventManagerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();