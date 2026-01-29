namespace Conda_AI.AIAPI.OpenAi;

public class OpenAiResponse
{
    public string id { get; set; }
    public string @object { get; set; }
    public long created_at { get; set; }
    public string status { get; set; }
    public bool background { get; set; }
    public string error { get; set; }
    public string model { get; set; }
    public List<OpenAiOutput> output { get; set; }
    // Add other fields as needed
}

public class OpenAiOutput
{
    public string id { get; set; }
    public string type { get; set; }
    public string status { get; set; }
    public string role { get; set; }
    public List<OpenAiOutputContent> content { get; set; }
}

public class OpenAiOutputContent
{
    public string type { get; set; }
    public List<object> annotations { get; set; }
    public List<object> logprobs { get; set; }
    public string text { get; set; }
}