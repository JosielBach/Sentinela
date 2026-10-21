namespace Sentinela.Agent.Configuration;

public class AgentConfiguration
{
    public string ApiUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public TimeSpan Interval { get; init; }
    public int BufferLimit { get; init; }
}
