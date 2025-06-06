using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using ReckonApp.Infrastructure.ExternalApi.Models;
using ReckonApp.Models;

namespace ReckonApp.Infrastructure.ExternalApi
{
    public class ReckonApiClient : IReckonApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ReckonApiSettings _settings;

        public ReckonApiClient(HttpClient httpClient, IOptions<ReckonApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<SubTextsResult> GetSubTextsAsync()
        {
            if (string.IsNullOrEmpty(_settings.SubTextsUrl))
                throw new InvalidOperationException("SubTextsUrl is not configured.");

            var response = await _httpClient.GetAsync(_settings.SubTextsUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var subTexts = JsonSerializer.Deserialize<SubTextsResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return subTexts ?? throw new Exception("Failed to deserialize subtexts from API response.");
        }

        public async Task<StringToSearchResult> GetTextToSearchAsync()
        {
            if (string.IsNullOrEmpty(_settings.TextToSearchUrl))
                throw new InvalidOperationException("TextToSearchUrl is not configured.");

            var response = await _httpClient.GetAsync(_settings.TextToSearchUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var stringToSearch = JsonSerializer.Deserialize<StringToSearchResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return stringToSearch ?? throw new Exception("Failed to retrieve text to search from API response.");
        }

        public async Task<bool> PostSubmitResultsAsync(SubmitResultsModel data)
        {
            if (string.IsNullOrEmpty(_settings.SubmitResultsUrl))
                throw new InvalidOperationException("SubmitResultsUrl is not configured.");

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(_settings.SubmitResultsUrl, content);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
    }
}
