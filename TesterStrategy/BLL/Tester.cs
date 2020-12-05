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
            ILoader parser)
        {
            _parser = parser;
            _logger = logger;
        }

        public async Task Run(CancellationToken token = default)
        {
            var filenameRts920 = "RTS-9.20_H1.csv";
            // var filenameRTS1220 = "RTS-12.20_M1.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Sources", filenameRts920);

            var bars = await _parser.LoadBars(filePath, token);

            var market = new Market(bars);
            market.SetSymbol(options =>
            {
                options.Name = "RTS-9.20";
                options.Margin = 25000;
                options.PriceStep = 15.50;
                options.PipsStep = 10;
            });
            
            var trader = new Trader(market, market);

            trader.Configuration(config =>
            {
                config.Balance = 100000;
                config.Strategies = new Strategy[]
                {
                    new MovingAverageStrategy(50)
                };
            });

            market.Emulate();

            _logger.LogInformation($"Эмуляция закончена. Баланс {trader.Balance}.");
        }
    }
}
