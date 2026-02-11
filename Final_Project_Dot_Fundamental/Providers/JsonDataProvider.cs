using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Final_Project_Dot_Fundamental.Providers
{
    internal class JsonDataProvider<T> : IDataProvider<T>
    {
        private readonly ILogger<JsonDataProvider<T>> _logger;

        public JsonDataProvider(ILogger<JsonDataProvider<T>> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ReadAsync(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogError("JSON file not found: {Path}", path);
                    throw new FileNotFoundException($"JSON file not found: {path}");
                }

                await using var stream = File.OpenRead(path);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, options);

                if (data == null)
                {
                    _logger.LogWarning("No data found in JSON file");
                    return Enumerable.Empty<T>();
                }

                return data;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON file");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading JSON file");
                throw;
            }
        }
    }
}
