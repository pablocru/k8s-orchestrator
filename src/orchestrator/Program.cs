var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
  var message = "Hello World!";

  logger.LogInformation("Message: {Message}", message);
  return message;
});

app.Run();
