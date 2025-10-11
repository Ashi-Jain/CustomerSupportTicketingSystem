using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

// Add this BEFORE Ocelot
app.MapGet("/", () => Results.Ok("Ocelot Gateway is running ✅"));

app.Run();
