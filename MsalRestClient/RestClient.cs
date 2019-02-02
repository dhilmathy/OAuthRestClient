using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MsalRestClient
{
    public class RestClient
    {
        private readonly AuthHelper authHelper;
        private readonly HttpClient httpClient;
        private readonly IJsonSerializer jsonSerializer;

        public RestClient(IConfigProvider configProvider, IJsonSerializer serializer)
        {
            authHelper = new AuthHelper(configProvider);
            jsonSerializer = serializer;

            httpClient = new HttpClient { BaseAddress = configProvider.GetBaseUrl() };
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        }
        private async Task AcquireTokenAsync()
        {
            var token = await authHelper.GetTokenAsync().ConfigureAwait(false);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> GetAsync(string resource, Dictionary<string, string> parameters = null)
        {
            var urlWithParameters = resource;
            if (parameters != null && parameters.Any())
            {
                var queryParams = string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
                urlWithParameters = $"{urlWithParameters}?{queryParams}";
            }
            var request = new HttpRequestMessage(HttpMethod.Get, urlWithParameters);
            await AcquireTokenAsync();

            var response = await httpClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<T> GetAsync<T>(string resource, Dictionary<string, string> parameters = null)
        {
            var jsonResult = await GetAsync(resource, parameters);
            return jsonSerializer.Deserialize<T>(jsonResult);
        }

        public async Task<T> PostAsync<T>(string resource, string jsonContent, Dictionary<string, string> parameters = null)
        {
            var jsonPath = resource.Split('/')[0];
            var urlWithParameters = resource;

            if (parameters != null && parameters.Any())
            {
                var queryParams = string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
                urlWithParameters = $"{urlWithParameters}?{queryParams}";
            }

            var postContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            await AcquireTokenAsync();
            var response = await httpClient.PostAsync(urlWithParameters, postContent);

            var jsonResult = await response.Content.ReadAsStringAsync();
            return jsonSerializer.Deserialize<T>(jsonResult);
        }
    }
}
