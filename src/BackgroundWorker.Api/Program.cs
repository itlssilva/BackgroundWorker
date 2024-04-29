using BackgroundWorker.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddArchitectures();

var app = builder.Build();

app.UseArchitectures();
app.MapHealthChecks("/health");

app.Run();
