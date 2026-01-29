using Newtonsoft.Json;

namespace Conda_AI.AIAPI.Ollama
{
   public class OllamaApiMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
