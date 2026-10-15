var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();   // turn on support for controller classes

var app = builder.Build();

app.MapControllers();   // connect the URLs ([Route], [HttpGet]...) to the controller methods

app.Run();   // start the web server