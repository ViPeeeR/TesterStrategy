using System;
using System.Collections.Generic;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.BLL.Strategies;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Trader : ITrader
    {
        private const int DefaultBalance = 100000;

        private readonly IMarketInfo _marketInfo;
        private ITradeManager _tradeManager;
        private Strategy[] _strategies;

        public IReadOnlyList<Order> Orders => _tradeManager.GetOrders();

        public IReadOnlyList<Order> History => _tradeManager.GetHistory();

        public double Balance => _tradeManager.Balance;

        public double Margin => _tradeManager.Margin;


        public Trader(
            IStockCommunity stockCommunity,
            IMarketInfo marketInfo)
        {
            _marketInfo = marketInfo;
            stockCommunity.RegisterTrader(this);
            _tradeManager = new TradeManager(_marketInfo.SymbolInfo, DefaultBalance);
        }

        public void Configuration(Action<TraderOptions> config)
        {
            var traderOptions = new TraderOptions();
            config?.Invoke(traderOptions);
            if (traderOptions.Balance != null)
            {
                _tradeManager = new TradeManager(_marketInfo.SymbolInfo, traderOptions.Balance.Value);
            }

            _strategies = traderOptions.Strategies ?? Array.Empty<Strategy>();
            foreach (var strategy in _strategies)
            {
                strategy.Configuration(strategyOption =>
                {
                    strategyOption.TradeManager = _tradeManager;
                    strategyOption.Volume = 1;
                });
            }
        }

        public void Update(IChart chart)
        {
            // обновить профит у сделок, проверить ТП и СЛ
            _tradeManager.UpdateOrders(chart);

            // дернуть стратегии для проверки входа/выхода
            foreach (var strategy in _strategies)
            {
                strategy.Run(chart);
            }
        }
    }
}
