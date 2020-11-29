using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Tester : ITester
    {
        private readonly ILoader _parser;
        private readonly IMarket _market;
        private readonly ITrader _trader;
        private readonly ILogger<Tester> _logger;

        public Tester(
            ILogger<Tester> logger,
            ILoader parser,
            IMarket market,
            ITrader trader
            )
        {
            _parser = parser;
            _market = market;
            _trader = trader;
            _logger = logger;
        }

        public async Task Run(CancellationToken token = default)
        {
            var filenameRTS920 = "RTS-9.20_M1.csv";
            var filenameRTS1220 = "RTS-12.20_M1.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), filenameRTS920);

            var bars = await _parser.LoadBars(filePath, token);

            _logger.LogInformation($"File was load and contains {bars.LongLength} bars");

            _market.Configuration(options =>
            {
                options.Bars = bars;
                options.Margin = 25000;
                options.PriceStep = 15.50;
                options.PipsStep = 10;
            });

            _trader.Configuration(config =>
            {
                config.Balance = 100000;
                config.MarketInfo = _market.MarketInfo;
            });

            while (_market.Tick())
            {
                _trader.Update();

                var currentBar = _market.MarketInfo.Chart.GetBar(0);
                if (currentBar.Time == TimeSpan.FromHours(11))
                {
                    _trader.Trade(OrderType.Sell, currentBar.Open, 1, 2000, 1000);
                }
            }

            var balance = _trader.Balance;
            var margin = _trader.Margin;
            _logger.LogInformation($"График закончился. Баланс {balance}. Открыто сделок {_trader.Orders.Count}");
        }
    }
}
