namespace Conda_AI.AIAPI.Ollama;

public class UpdatePayload
{
    public Dictionary<string, object> Updates { get; set; }
    public UpdateTarget Target { get; set; }
}

public class UpdateTarget
{
    public MatchCondition Match { get; set; }
}

public class MatchCondition
{
    public string Field { get; set; }
    public string Operator { get; set; } // e.g., "ilike"
    public string Value { get; set; }
}