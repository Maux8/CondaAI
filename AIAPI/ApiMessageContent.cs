using Conda_AI.AIAPI.Ollama;
using Conda_AI.Model;
using Newtonsoft.Json;

namespace Conda_AI.AIAPI;

public class ApiMessageContent
{
    [JsonProperty("intent")]
    public ApiResponseIntent Intent { get; set; }
   
    [JsonProperty("answer")]
    public string? Answer { get; set; }
    
    [JsonProperty("task")]
    public TaskModel? Task { get; set; }
    
    [JsonProperty("update")]
    public UpdatePayload Update { get; set; }
}