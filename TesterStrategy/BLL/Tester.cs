using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.BLL.Strategies;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Tester : ITester
    {
        private readonly ILoader _parser;
        private readonly ILogger<Tester> _logger;

        public Tester(
            ILogger<Tester> logger,
            ILogger<Trader> traderLogger,
            ILoader parser)
        {
            _parser = parser;
            _logger = logger;
        }

        public async Task Run(CancellationToken token = default)
        {


            var filenameRTS920 = "RTS-9.20_H1.csv";
            var filenameRTS1220 = "RTS-12.20_M1.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), filenameRTS920);

            var bars = await _parser.LoadBars(filePath, token);

            var market = new Market();
            market.SetSymbol(options =>
            {
                options.Name = "RTS-9.20";
                options.Bars = bars;
                options.Margin = 25000;
                options.PriceStep = 15.50;
                options.PipsStep = 10;
            });
            var trader = new Trader(market, market);

            trader.Configuration(config =>
            {
                config.Balance = 100000;
                config.Strategies = new IStrategy[]
                {
                    new MovingAverageStrategy(50)
                };
            });

            var emaStrategy = new MovingAverageStrategy(_market.MarketInfo.Chart, trader, 50, _logger);

            while (market.Tick())
            {
                emaStrategy.Run();
            }

            trader.CloseOrders();

            _logger.LogInformation($"График закончился. Баланс { trader.Balance}.");
        }
    }
}
