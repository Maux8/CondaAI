using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Conda_AI.AIAPI
{
    public abstract class ApiRequestClient<TResponse>
    {
        protected readonly HttpClient HttpClient = new();
        protected abstract string GetBaseUri();

        protected abstract string GetModelName();
        
        protected ApiRequestClient()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected TResponse ParseResponse(string rawResult)
        {
            var result = JsonConvert.DeserializeObject<TResponse>(rawResult);
            
            if (result == null)
            {
                throw new ApiRequestException(
                    "The API returned an invalid response.",
                    HttpStatusCode.InternalServerError
                );
            }
            
            return result;
        }

        public async Task<TResponse> ExecuteRequest(string endpoint, ApiRequestOptions? options = null)
        {
            var body = new Dictionary<string, object> { { "model", GetModelName() } };

            if (options != null && options.ExtraOptions.Count != 0)
            {
                body = options.ExtraOptions
                    .Concat(body.Where(kvp => !options.ExtraOptions.ContainsKey(kvp.Key)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            if (!string.IsNullOrWhiteSpace(options?.Apikey)) 
            {
                HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.Apikey);
            }
            
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF32, "application/json");

            var fullUri = new Uri(new Uri(GetBaseUri()), endpoint);
            var response = await HttpClient.PostAsync(fullUri, content);
            var resContent = await response.Content.ReadAsStringAsync();
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Clipboard.SetTextAsync(resContent);
            });

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(resContent);
                return ParseResponse(resContent);
            }

            throw new ApiRequestException(
                resContent,
                response.StatusCode
            );
        }
    }
}