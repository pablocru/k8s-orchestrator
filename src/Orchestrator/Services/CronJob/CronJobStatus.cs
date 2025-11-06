namespace Orchestrator.Services.CronJob;

public enum CronJobStatus
{
  Idle,       // Service created but never started
  Running,    // Cron job is currently running
  Stopped     // Cron job was stopped
}
