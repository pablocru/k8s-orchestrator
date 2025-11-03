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

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Register Controllers
builder.Services.AddControllers();

// Register sample Task
builder.Services.AddSingleton<ICronJobTask, ExampleTask>();

// Register Cron Job Service
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
