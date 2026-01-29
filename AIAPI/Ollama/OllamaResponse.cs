using Newtonsoft.Json;

namespace Conda_AI.AIAPI.Ollama;

public class OllamaResponse
{
    [JsonProperty("model")] public string Model { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("message")] public OllamaApiMessage Message { get; set; }

    [JsonProperty("done")] public bool Done { get; set; }
}