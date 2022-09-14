using AE.CSV.Common.Application.Common.Interfaces;
using AE.CSV.Common.Infrastructure.Helper;
using AE.CSV.Common.Infrastructure.Utilities;
using AE.CSV.CSApp.Configs;
using AE.CSV.CSApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AE.CSV.CSApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("Application", "Console Application")
            .Enrich.WithProperty("UserId", "System Account")
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: AppDomain.CurrentDomain.BaseDirectory + "\\logs\\log_.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information,
                fileSizeLimitBytes: 10240000
            )
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                restrictedToMinimumLevel: LogEventLevel.Information
            ).CreateLogger();

        try
        {
            var host = CreateHostBuilder(args, configuration).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var consoleApp = services.GetRequiredService<IConsoleApp>();
                    await consoleApp.Run(args);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot Configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                var consoleAppConfig = Configuration.GetSection("ConsoleAppConfigSettings");
                services.Configure<ConsoleAppConfigSettings>(consoleAppConfig);

                services.AddSingleton<ICSVFileHelper, CSVFileHelper>();
                services.AddSingleton<IFileUtillity, FileUtillity>();

                services.AddSingleton<IConsoleApp, ConsoleApp>();
            });
}