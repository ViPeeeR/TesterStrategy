using Microsoft.Extensions.Logging;
using System;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Market : IMarket
    {
        private readonly ILogger<Market> _logger;
        private readonly MarketOptions _marketOptions;
        private MarketInfo _marketInfo;

        public Market(
          ILogger<Market> logger)
        {
            _logger = logger;
            _marketOptions = new MarketOptions();
        }

        public MarketInfo MarketInfo => _marketInfo;

        public void Configuration(Action<MarketOptions> options)
        {
            options?.Invoke(_marketOptions);
            _marketInfo = new MarketInfo(
                new Chart(_marketOptions.Bars),
                _marketOptions.Margin,
                _marketOptions.PriceStep,
                _marketOptions.PipsStep);
        }

        public bool Tick()
        {
            return _marketInfo.Chart?.Next() != null;
        }
    }
}
