using Microsoft.AspNetCore.Mvc;

namespace orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/job (JobController = job; [controller] = job)
public class JobController(ILogger<JobController> logger) : ControllerBase
{
  [HttpGet("start")]
  public IActionResult StartContinuousJob()
  {
    logger.LogInformation("Starting Cron Job via API call...");

    var message = "Cron Job started successfully";
    logger.LogInformation("{Message}", message);
    return Ok(new { message });
  }

  [HttpGet("stop")]
  public IActionResult StopContinuousJob()
  {
    logger.LogInformation("Stopping Cron Job via API call...");

    var message = "Cron Job stopped successfully";
    logger.LogInformation("{Message}", message);
    return Ok(new { message });
  }

  [HttpGet("run-once")]
  public IActionResult RunJobOnce()
  {
    logger.LogInformation("Running Job via API call...");

    var message = "Job ran successfully";
    logger.LogInformation("{Message}", message);
    return Ok(new { message });
  }
}
