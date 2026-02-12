using Final_Project_Dot_Fundamental.Model;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Final_Project_Dot_Fundamental.Providers
{
    internal class CsvDataProvider<T> : IDataProvider<T> 
    {
        private readonly ILogger<CsvDataProvider<T>> _logger;

        public CsvDataProvider(ILogger<CsvDataProvider<T>> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<T>> ReadAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Reading CSV file from {Path}", path);

                if (!File.Exists(path))
                {
                    _logger.LogError("CSV file not found: {Path}", path);
                    throw new FileNotFoundException($"CSV file not found: {path}");
                }
                cancellationToken.ThrowIfCancellationRequested();

                using var reader = new StreamReader(path);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                    BadDataFound = context =>
                    {
                        _logger.LogWarning("Bad data found at row {Row}: {RawRecord}",
                            context.Context.Parser.Row,
                            context.RawRecord);
                    },
                    MissingFieldFound = null,
                    ReadingExceptionOccurred = ex =>
                    {
                        _logger.LogWarning(ex.Exception.Message);
                        return false;
                    }
                };
                using var csv = new CsvReader(reader,config);

                var records = csv.GetRecords<T>().ToList();

                if (records.Count == 0)
                {
                    _logger.LogWarning("No data found in CSV file");
                }
                else
                {
                    _logger.LogInformation("Successfully read {Count} records from CSV", records.Count);
                }
                return Task.FromResult<IEnumerable<T>>(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV file: {Path}", path);
                throw;
            }
        }
    }
}
