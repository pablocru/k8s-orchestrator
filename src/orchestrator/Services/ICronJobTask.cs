namespace Orchestrator.Services;

public interface ICronJobTask
{
  Task ExecuteAsync(CancellationToken cancellationToken);
}
