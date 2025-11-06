namespace Orchestrator.Services.CronJob;

public interface ICronJobService
{
  bool Start();
  bool Stop();
  Task<bool> RunOnceAsync();
  CronJobStatus GetStatus();
}
