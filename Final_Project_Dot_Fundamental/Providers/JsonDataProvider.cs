using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Final_Project_Dot_Fundamental.Providers
{
    internal class JsonDataProvider<T> : IDataProvider<T>
    {
        private readonly ILogger<JsonDataProvider<T>> _logger;

        public JsonDataProvider(ILogger<JsonDataProvider<T>> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ReadAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogError("JSON file not found: {Path}", path);
                    throw new FileNotFoundException($"JSON file not found: {path}");
                }

                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    Error = (sender, args) =>
                    {
                        _logger.LogError(
                            "Bug at {Member} - Error: {Message}",
                            args.ErrorContext.Member,
                            args.ErrorContext.Error.Message
                        );
                        args.ErrorContext.Handled = true; 
                    }
                };

                using var reader = new StreamReader(path);
                var json = await reader.ReadToEndAsync();

                cancellationToken.ThrowIfCancellationRequested();

                var data = JsonConvert.DeserializeObject<List<T>>(json, settings);

                if (data == null)
                {
                    _logger.LogWarning("No data found in JSON file");
                    return Enumerable.Empty<T>();
                }

                _logger.LogInformation("Read {Count} records from JSON file", data.Count);
                return data;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON file");
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Reading JSON file was cancelled");
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
