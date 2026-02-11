using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Final_Project_Dot_Fundamental.Providers
{
    internal class ApiDataProvider<T> : IDataProvider<T>
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger<ApiDataProvider<T>> _logger;

        public ApiDataProvider(ILogger<ApiDataProvider<T>> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ReadAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("URL cannot be empty");
                throw new ArgumentException("URL cannot be empty", nameof(url));
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? new List<T>();
        }
    }
}
