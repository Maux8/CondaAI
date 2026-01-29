using Conda_AI.AIAPI;
using Conda_AI.AIAPI.Ollama;

namespace Conda_AI.Service
{
    /// <summary>
    /// Dienstklasse zur Kommunikation mit der Ollama-API über eine HTTPS-Endpunkt.
    /// Erbt von <see cref="ApiRequestClient{TResponse}"/> mit dem spezifizierten Antworttyp <see cref="OllamaResponse"/>.
    /// </summary>
    public class OllamaApiService : ApiRequestClient<OllamaResponse>
    {
        /// <summary>
        /// Gibt die Basis-URI für die Ollama-API zurück.
        /// </summary>
        /// <returns>Die vollständige URI als <see cref="string"/>.</returns>
        protected override string GetBaseUri() => "https://f2ki-h100-1.f2.htw-berlin.de:11435";

        /// <summary>
        /// Gibt den Namen des verwendeten KI-Modells zurück.
        /// </summary>
        /// <returns>Der Modellname als <see cref="string"/>.</returns>
        protected override string GetModelName() => "deepseek-r1:8b";
    }
}
