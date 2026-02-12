using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Final_Project_Dot_Fundamental.Providers
{
    internal class ApiDataProvider<T> : IDataProvider<T>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiDataProvider<T>> _logger;

        public ApiDataProvider(IHttpClientFactory httpClientFactory,ILogger<ApiDataProvider<T>> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ReadAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("URL cannot be empty");
                throw new ArgumentException("URL cannot be empty", nameof(url));
            }
            _logger.LogInformation("Calling API: {Url}", url);
            cancellationToken.ThrowIfCancellationRequested();
            HttpClient client = _httpClientFactory.CreateClient();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                var response = await client.GetFromJsonAsync<IEnumerable<T>>(url, jsonOptions);
                _logger.LogInformation("Successfully retrieved data from API");
                return response ?? Array.Empty<T>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error ");
                throw;
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError(ex, "The content type is not supported");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error ");
                throw;
            }

        }
    }
}
