using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Exceptions;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Market : IMarket, IStockCommunity, IMarketInfo
    {
        private readonly ICollection<ITrader> _traders;

        private readonly IChartManager _chartManager;

        public SymbolInfo SymbolInfo { get; private set; }

        public Market(Bar[] bars)
        {
            _chartManager = new ChartManager(bars);
            _traders = new List<ITrader>();
        }

        public void SetSymbol(Action<SymbolOptions> config)
        {
            var symbolOptions = new SymbolOptions();
            config?.Invoke(symbolOptions);

            SymbolInfo = new SymbolInfo(
                symbolOptions.Name,
                symbolOptions.Margin,
                symbolOptions.PriceStep,
                symbolOptions.PipsStep);
        }

        public void Emulate()
        {
            while (Tick())
            {
                // тут можно выводить лог, если надо, что идет график, либо задержку делать
            }

            // TODO: Закрыть все не закрытые сделки
        }

        public void RegisterTrader(ITrader trader)
        {
            _traders.Add(trader);
        }

        public void Notify()
        {
            foreach (var trader in _traders)
            {
                trader.Update(_chartManager.Chart);
            }
        }

        private bool Tick()
        {
            var isUpdate = _chartManager.Next();
            Notify();
            return isUpdate;
        }
    }
}