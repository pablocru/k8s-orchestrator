namespace Orchestrator.Configuration;

public class CronJobSettings
{
  public TimeSpan StartHour { get; set; }
  public TimeSpan EndHour { get; set; }
  public TimeSpan TaskInterval { get; set; }
}
