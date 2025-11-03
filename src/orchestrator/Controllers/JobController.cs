using Microsoft.AspNetCore.Mvc;
using Orchestrator.Services;

namespace Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/job (JobController = job; [controller] = job)
public class JobController : ControllerBase
{
  private readonly ILogger<JobController> _logger;
  private readonly ICronJobService _cronService;

  public JobController(ILogger<JobController> logger, ICronJobService cronService)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _cronService = cronService ?? throw new ArgumentNullException(nameof(cronService));
  }

  [HttpGet("start")]
  public IActionResult StartContinuousJob()
  {
    _logger.LogInformation("Starting CronJob via API call...");

    if (_cronService.Start())
    {
      var message = "CronJob started successfully";
      _logger.LogInformation("{Message}", message);
      return Ok(new { message });
    }
    else
    {
      var message = "CronJob is already running";
      _logger.LogWarning("{Message}", message);
      return BadRequest(new { message });
    }
  }

  [HttpGet("stop")]
  public IActionResult StopContinuousJob()
  {
    _logger.LogInformation("Stopping CronJob via API call...");

    if (_cronService.Stop())
    {
      var message = "CronJob stopped successfully";
      _logger.LogInformation("{Message}", message);
      return Ok(new { message });
    }
    else
    {
      var message = "CronJob was not running";
      _logger.LogWarning("{Message}", message);
      return BadRequest(new { message });
    }
  }

  [HttpGet("run-once")]
  public async Task<IActionResult> RunJobOnce()
  {
    _logger.LogInformation("Running Job via API call...");

    if (await _cronService.RunOnceAsync())
    {
      var message = "Job ran successfully";
      _logger.LogInformation("{Message}", message);
      return Ok(new { message });
    }
    else
    {
      var message = "Job could not run because CronJob is already running";
      _logger.LogWarning("{Message}", message);
      return BadRequest(new { message });
    }
  }

  [HttpGet("status")]
  public IActionResult GetStatus()
  {
    var status = _cronService.GetStatus();
    _logger.LogInformation("CronJob status requested: {Status}", status);
    return Ok(new { status = status.ToString() });
  }
}
