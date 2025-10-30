var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => {
  var message = "Hello World!";

  Console.WriteLine(message);
  return message;
});

app.Run();
