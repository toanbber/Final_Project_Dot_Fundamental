using Final_Project_Dot_Fundamental.Model;
using Final_Project_Dot_Fundamental.Processor;
using Final_Project_Dot_Fundamental.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace Final_Project_Dot_Fundamental
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddSerilog(dispose: true));
            services.AddHttpClient();

            string jsonPath = config["DataSources:Todo:BadJsonPath"];
            string csvPath = config["DataSources:Todo:BadCsvPath"];
            string apiUrl = config["DataSources:Todo:ApiPath"];

            services.AddTransient<JsonDataProvider<Todo>>();
            services.AddTransient<CsvDataProvider<Todo>>();
            services.AddTransient<ApiDataProvider<Todo>>();
            services.AddTransient<TodoProcessor>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Application starting");
            var stopwatch = new Stopwatch();
            var startTime = DateTime.Now;
            stopwatch.Start();

            logger.LogInformation("Start time: {StartTime}", startTime);
            try
            {
                var todoProcessor = serviceProvider.GetRequiredService<TodoProcessor>();

                var jsonProvider = serviceProvider.GetRequiredService<JsonDataProvider<Todo>>();
                var csvProvider = serviceProvider.GetRequiredService<CsvDataProvider<Todo>>();
                var apiProvider = serviceProvider.GetRequiredService<ApiDataProvider<Todo>>();

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true; 
                    cts.Cancel();
                };

                logger.LogInformation("Start reading from json");
                var jsonTodos = await jsonProvider.ReadAsync(jsonPath, cts.Token);
                 todoProcessor.ProcessData(jsonTodos);
                logger.LogInformation("Finished reading from json");

                logger.LogInformation("Start reading from CSV");
                var csvTodos = await csvProvider.ReadAsync(csvPath,cts.Token);
                todoProcessor.ProcessData(csvTodos);
                logger.LogInformation("Finished reading from CSV");

                logger.LogInformation("Start reading Todo data from API: {Url}", apiUrl);
                var apiTodos = await apiProvider.ReadAsync(apiUrl,cts.Token);
                todoProcessor.ProcessData(apiTodos);
                logger.LogInformation("Finished processing API Todo data");

                var allTodos = jsonTodos
                    .Concat(csvTodos)
                    .Concat(apiTodos)
                    .ToList();

                var outputPath = "merged_todos.csv";
                logger.LogInformation("Exporting merged data to CSV: {Path}", outputPath);
                todoProcessor.ExportToCsv(allTodos, outputPath);
                stopwatch.Stop();
                var endTime = DateTime.Now;

                logger.LogInformation("End time: {EndTime}", endTime);
                logger.LogInformation("Total execution time: {Ms}", stopwatch.ElapsedMilliseconds);
                logger.LogInformation("Total records processed: {Count}", allTodos.Count);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Operation was cancelled by user.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
            }

            finally
            {
                Log.CloseAndFlush();
            }

            Console.ReadKey();
        }
    }
}