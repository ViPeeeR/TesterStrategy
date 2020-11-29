using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;
using TesterStrategy.BLL;
using TesterStrategy.BLL.Interfaces;

namespace TesterStrategy
{
    public class Program
    {
        public static ILoggerFactory LoggerFactory;
        public static IConfigurationRoot Configuration;

        static async Task Main(string[] args)
        {
            //string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true);
                //.AddJsonFile($"appsettings.{environment}.json", optional: false);

            Configuration = builder.Build();
            
            var serilog = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration) // все настройки логирования в конфигах
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.AddSerilog(serilog, true))
                .ConfigurationService()
                .BuildServiceProvider();

            LoggerFactory = serviceProvider
                .GetService<ILoggerFactory>();

            var tester = serviceProvider.GetService<ITester>();
            await tester.Run();

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
