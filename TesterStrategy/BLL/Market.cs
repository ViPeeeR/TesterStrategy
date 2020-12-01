using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Market : IMarket, IStockCommunity, IMarketInfo
    {
        private readonly ICollection<ITrader> _traders;
        public SymbolInfo SymbolInfo { get; private set; }

        public Market()
        {
            _traders = new List<ITrader>();
        }

        public void SetSymbol(Action<SymbolOptions> config)
        {
            var symbolOptions = new SymbolOptions();
            config?.Invoke(symbolOptions);

            SymbolInfo = new SymbolInfo(
                symbolOptions.Name,
                symbolOptions.Bars,
                symbolOptions.Margin,
                symbolOptions.PriceStep,
                symbolOptions.PipsStep);
        }

        public bool Tick()
        {
            var isUpdate = SymbolInfo.Chart?.Next() != null;
            Notify();
            return isUpdate;
        }

        public void RegisterTrader(ITrader trader)
        {
            _traders.Add(trader);
        }

        public void Notify()
        {
            foreach (var trader in _traders)
            {
                trader.Update();
            }
        }
    }
}
