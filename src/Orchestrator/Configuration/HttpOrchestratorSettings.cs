namespace Orchestrator.Configuration;

public class EndpointConfig
{
  public string Name { get; set; } = string.Empty;
  public string Path { get; set; } = string.Empty;
}

public class HttpOrchestratorSettings
{
  public string Host { get; set; } = string.Empty;
  public int Port { get; set; }
  public List<EndpointConfig> Endpoints { get; set; } = new();
}

