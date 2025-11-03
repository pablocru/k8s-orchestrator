namespace Orchestrator.Services;

public interface ICronJobService
{
  bool Start();
  bool Stop();
  Task<bool> RunOnceAsync();
  CronJobStatus GetStatus();
}
