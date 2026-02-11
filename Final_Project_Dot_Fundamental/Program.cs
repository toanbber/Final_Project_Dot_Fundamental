using Final_Project_Dot_Fundamental.Model;
using Final_Project_Dot_Fundamental.Processor;
using Final_Project_Dot_Fundamental.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            string jsonPath = config["DataSources:Titanic:JsonPath"];
            string csvPath = config["DataSources:Titanic:CsvPath"];
            string apiUrl = config["DataSources:Api:TodoUrl"];

            services.AddTransient<IDataProvider<Titanic>, JsonDataProvider<Titanic>>();
            services.AddTransient<IDataProvider<Titanic>, CsvDataProvider<Titanic>>();
            services.AddTransient<IDataProvider<Todo>, ApiDataProvider<Todo>>();
            services.AddTransient<TitanicDataProcessor>();
            services.AddTransient<TodoProcessor>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Application starting");

            try
            {
                var titanicProcessor = serviceProvider.GetRequiredService<TitanicDataProcessor>();
                var todoProcessor = serviceProvider.GetRequiredService<TodoProcessor>();
                var titanicProviders = serviceProvider.GetServices<IDataProvider<Titanic>>();
                var todoProvider = serviceProvider.GetRequiredService<IDataProvider<Todo>>();

                foreach (var provider in titanicProviders)
                {
                    string path = provider is JsonDataProvider<Titanic> ? jsonPath : csvPath;
                    logger.LogInformation("Start reading Titanic data");
                    var passengers = await provider.ReadAsync(path);
                    logger.LogInformation("Finished reading Titanic data");
                    logger.LogInformation("Start processing Titanic data");
                    titanicProcessor.ProcessData(passengers);
                    logger.LogInformation("Finished processing Titanic data");
                }

             
                logger.LogInformation("Start reading Todo data");
                var todos = await todoProvider.ReadAsync(apiUrl);
                logger.LogInformation("Finished reading Todo data");
                logger.LogInformation("Start processing Todo data");
                todoProcessor.ProcessData(todos);
                logger.LogInformation("Finished processing Todo data");
                logger.LogInformation("=== Application finished successfully ===");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error");
            }
            Console.ReadKey();
        }
    }
}
