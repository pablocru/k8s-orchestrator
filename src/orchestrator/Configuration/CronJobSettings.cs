using System.ComponentModel.DataAnnotations;

namespace Orchestrator.Configuration;

public class CronJobSettings
{
  [Range(0, 23)]
  public int StartHour { get; set; }

  [Range(0, 23)]
  public int EndHour { get; set; }

  [Range(1, 1440)]
  public int TaskIntervalMinutes { get; set; }
}

