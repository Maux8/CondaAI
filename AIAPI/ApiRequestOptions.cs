namespace Conda_AI.AIAPI
{

    public class ApiRequestOptions
    {
        public string? Apikey { get; set; }
        public Dictionary<string, object> ExtraOptions { get; set; } = new();
    }
}
