namespace Orchestrator.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class CronJobService(
  ILogger<CronJobService> logger,
  ICronJobTask task,
  TimeSpan startHour,
  TimeSpan endHour,
  TimeSpan taskInterval) : ICronJobService, IHostedService
{
  private readonly ILogger<CronJobService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly ICronJobTask _task = task ?? throw new ArgumentNullException(nameof(task));
  private CancellationTokenSource? _cts;
  private Task? _runningTask;
  private CronJobStatus _status = CronJobStatus.Idle;
  private readonly TimeSpan _startHour = startHour;
  private readonly TimeSpan _endHour = endHour;
  private readonly TimeSpan _taskInterval = taskInterval;

  public bool Start()
  {
    if (_runningTask != null && !_runningTask.IsCompleted)
    {
      _logger.LogWarning("Attempted to start CronJob but it is already running.");
      return false;
    }

    _logger.LogInformation("Starting CronJob...");
    _cts = new CancellationTokenSource();
    _status = CronJobStatus.Running;
    _runningTask = Task.Run(() => RunCronAsync(_cts.Token));
    return true;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("API started. Auto-starting CronJob...");
    Start();
    return Task.CompletedTask;
  }

  public bool Stop()
  {
    if (_runningTask == null || _runningTask.IsCompleted)
    {
      _logger.LogWarning("Attempted to stop CronJob but it was not running.");
      return false;
    }

    _logger.LogInformation("Stopping CronJob...");
    _cts?.Cancel();
    _status = CronJobStatus.Stopped;
    return true;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("API stopping. Auto-stopping CronJob...");
    Stop();
    return Task.CompletedTask;
  }

  public async Task<bool> RunOnceAsync()
  {
    if (_status == CronJobStatus.Running)
    {
      _logger.LogWarning("RunOnceAsync was called but CronJob is already running. Skipping execution.");
      return false;
    }

    _logger.LogInformation("Executing task once without starting CronJob...");
    using var cts = new CancellationTokenSource();
    await _task.ExecuteAsync(cts.Token);
    return true;
  }

  public CronJobStatus GetStatus() => _status;

  private async Task RunCronAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("CronJob loop started. Status: {Status}", _status);

    while (!cancellationToken.IsCancellationRequested)
    {
      try
      {
        var now = DateTime.Now;
        TimeSpan delay;

        if (now.TimeOfDay >= _startHour && now.TimeOfDay <= _endHour)
        {
          try
          {
            _logger.LogInformation("Executing scheduled task at {Time}.", now);
            await _task.ExecuteAsync(cancellationToken);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Error executing scheduled task.");
          }

          var nextExecution = now.Add(_taskInterval);
          _logger.LogInformation(
            "Waiting {Interval} before next execution at {NextExecution}.",
            _taskInterval,
            nextExecution
          );
          delay = _taskInterval;
        }
        else
        {
          var nextStart = now.Date.AddDays(now.TimeOfDay < _startHour ? 0 : 1).Add(_startHour);
          delay = nextStart - now;

          _logger.LogInformation(
            "Outside working hours. Waiting until {NextStart}.",
            nextStart
          );
        }

        await Task.Delay(delay, cancellationToken);
      }
      catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        _logger.LogInformation("CronJob loop canceled due to Stop() request.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error in CronJob loop iteration.");
      }
    }

    _logger.LogInformation("CronJob loop stopped. Status: {Status}", _status);
  }
}
