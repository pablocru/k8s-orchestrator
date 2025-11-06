namespace Orchestrator.Services.Tasks;

public interface ICronJobTask
{
  Task ExecuteAsync(CancellationToken cancellationToken);
}
