using Microsoft.Extensions.Options;
using Orchestrator.Configuration;
using System.Diagnostics;

namespace Orchestrator.Services.Tasks;

public class HttpOrchestratorTask(
    ILogger<HttpOrchestratorTask> logger,
    IHttpClientFactory httpClientFactory,
    IOptions<HttpOrchestratorSettings> settings) : ICronJobTask
{
  private readonly ILogger<HttpOrchestratorTask> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
  private readonly HttpOrchestratorSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

  public async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    var stopwatch = Stopwatch.StartNew();
    _logger.LogInformation("Starting orchestration task at {Time}", DateTime.Now);

    if (_settings.Endpoints == null || _settings.Endpoints.Count == 0)
    {
      _logger.LogWarning("No endpoints configured. Task will not execute any requests.");
      return;
    }

    var messages = new List<string>();

    foreach (var endpoint in _settings.Endpoints)
    {
      var url = $"{_settings.Host}:{_settings.Port}{endpoint.Path}";
      var message = await GetMessageFromEndpointAsync(url, cancellationToken);

      messages.Add($"{endpoint.Name}: {message}");
    }

    _logger.LogInformation("Collected messages:");
    foreach (var msg in messages)
    {
      _logger.LogInformation("- {Message}", msg);
    }

    stopwatch.Stop();
    _logger.LogInformation(
      "Orchestration task finished at {Time} (Duration: {Duration} ms)",
      DateTime.Now, stopwatch.ElapsedMilliseconds
    );
  }

  private async Task<string> GetMessageFromEndpointAsync(string url, CancellationToken cancellationToken)
  {
    try
    {
      var response = await _httpClient.GetAsync(url, cancellationToken);
      var content = await response.Content.ReadAsStringAsync(cancellationToken);

      return $"HTTP {(int)response.StatusCode} ({response.StatusCode}) - {content}";
    }
    catch (HttpRequestException ex)
    {
      return $"Network error calling {url}: {ex.Message}";
    }
    catch (OperationCanceledException)
    {
      return $"HTTP request to {url} canceled because CronJob was stopped";
    }
    catch (Exception ex)
    {
      return $"Error calling {url}: {ex.Message}";
    }
  }
}
