// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TemporalTableTest.Repository;

namespace TemporalTableTest
{
    class Program
    {
        protected Program() { }

        public static IConfiguration Configuration { get; private set; } = default!;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(Program.Configuration)
            .RegisterAppDbContext(Configuration)
            .BuildServiceProvider();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();

            logger.LogDebug("Starting application");

            using var scope = serviceProvider.CreateScope();

            try
            {
                // Init Database
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                var shouldRestDatabaseOnRestart = Configuration.GetValue<bool>("ShouldClearDatabaseOnRestart");
                await initialiser.InitialiseAsync(shouldRestDatabaseOnRestart);

                var history = initialiser.GetHistory();
                //foreach (var item in history)
                //{

                //}

                Console.WriteLine("All Done");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
            }
        }
    }
}



