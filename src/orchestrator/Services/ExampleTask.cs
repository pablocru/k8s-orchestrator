namespace Orchestrator.Services;

public class ExampleTask(ILogger<ExampleTask> logger) : ICronJobTask
{
  private readonly ILogger<ExampleTask> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running task at {Time}.", DateTime.Now);
    await Task.Delay(2000, cancellationToken);
  }
}
