using Microsoft.Extensions.DependencyInjection;
using TesterStrategy.BLL;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services;
using TesterStrategy.BLL.Services.Interfaces;

namespace TesterStrategy
{
    public static class ServiceCollectionConfiguration
    {
        public static IServiceCollection ConfigurationService(this IServiceCollection services)
        {
            services.AddTransient<ILoader, CsvLoader>();
            services.AddTransient<ITester, Tester>();
            //services.AddTransient<IChart, Chart>();

            //services.AddSingleton<IMarket, Market>();
            //services.AddSingleton<ITrader, Trader>();

            return services;
        }
    }
}
