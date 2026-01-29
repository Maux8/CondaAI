using Conda_AI.AIAPI;
using Conda_AI.AIAPI.OpenAi;

namespace Conda_AI.Service;

public class OpenAiService: ApiRequestClient<OpenAiResponse>
{
    protected override string GetBaseUri() => "https://api.openai.com";

    protected override string GetModelName() => "gpt-4.1";
}