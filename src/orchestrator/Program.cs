using Microsoft.Extensions.Options;
using Orchestrator.Services;
using Orchestrator.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Validate appsettings.json
builder.Services.AddOptions<CronJobSettings>()
  .Bind(builder.Configuration.GetSection("CronJobSettings"))
  .ValidateDataAnnotations()
  .ValidateOnStart();
builder.Services.AddOptions<HttpOrchestratorSettings>()
  .Bind(builder.Configuration.GetSection("HttpOrchestratorSettings"))
  .ValidateOnStart();

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Register Controllers
builder.Services.AddControllers();

// Register HTTP Client & Orchestrator Task as a Singleton because only one is needed
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICronJobTask, HttpOrchestratorTask>();

// Register Cron Job Service also as a Singleton because only one is needed
builder.Services.AddSingleton<ICronJobService>(sp =>
{
  var logger = sp.GetRequiredService<ILogger<CronJobService>>();
  var task = sp.GetRequiredService<ICronJobTask>();
  var settings = sp.GetRequiredService<IOptions<CronJobSettings>>().Value;

  return new CronJobService(
    logger,
    task,
    TimeSpan.FromHours(settings.StartHour),
    TimeSpan.FromHours(settings.EndHour),
    TimeSpan.FromMinutes(settings.TaskIntervalMinutes)
  );
});

// Register Cron Job as a HostedService to auto-start
builder.Services.AddHostedService(sp => (CronJobService)sp.GetRequiredService<ICronJobService>());

// Run app
var app = builder.Build();
app.MapControllers();
app.Run();
