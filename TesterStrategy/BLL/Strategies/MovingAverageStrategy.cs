using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using TesterStrategy.BLL.Indicators;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Strategies
{
    public class MovingAverageStrategy : IStrategy
    {
        private readonly int _period;
        private ITradeManager _tradeManager;

        private int _volumeBase = 1;
        private SymbolInfo _symbol;
        private int _magicNumber;

        public MovingAverageStrategy(
            int period)
        {
            _period = period;
            _magicNumber = new Random().Next(1000, 9999);
        }

        public void Configuration(Action<StrategyOption> config)
        {
            var options = new StrategyOption();
            config?.Invoke(options);
            _tradeManager = options.TradeManager;
            _volumeBase = options.Volume ?? 1;
            _symbol = options.Symbol;
        }

        public bool Run()
        {
            var movingAverage = new MovingAverage(_symbol.Chart, _period);
            var ema = movingAverage.EmaValue();
            if (ema.Length == 0)
                return false;

            if (_tradeManager.GetOrders(_magicNumber).Any())
            {
                // TODO: Close check
                ClosePosition(ema);
            }

            if (!_tradeManager.GetOrders(_magicNumber).Any())
            {
                // TODO: Open check
                OpenPosition(ema);
            }

            return true;
        }

        private void ClosePosition(double[] ema)
        {
            var currentEma = ema[1];
            var previousBar = _symbol.Chart.GetBar(1);
            var currentBar = _symbol.Chart.GetBar(0);
            var volume = _volumeBase;

            var position = _tradeManager.GetOrders(_magicNumber).Select(x => x.Type).First();
            if (position == OrderType.Buy && previousBar.Open > currentEma && previousBar.Close < currentEma)
            {
                _tradeManager.Close(currentBar.Open);
            }

            if (position == OrderType.Sell && previousBar.Open < currentEma && previousBar.Close > currentEma)
            {
                _tradeManager.Close(currentBar.Open);
            }
        }

        private void OpenPosition(double[] ema)
        {
            var currentEma = ema[1];
            var previousBar = _symbol.Chart.GetBar(1);
            var currentBar = _symbol.Chart.GetBar(0);
            if (previousBar.Open > currentEma && previousBar.Close < currentEma)
            {
                // TODO: SELL
                _tradeManager.Open(OrderType.Sell, currentBar.Open, _volumeBase, magicNumber: _magicNumber);
                //_trader.Trade(OrderType.Sell, currentBar.Open, volume);
                Console.WriteLine($"Открываю позицию {OrderType.Sell.ToString()} с ema = {currentEma:F1}, дата {currentBar.Key}, цена {currentBar.Open}");
            }
            else
            {
                if (previousBar.Open < currentEma && previousBar.Close > currentEma)
                {
                    // TODO: Buy
                    _tradeManager.Open(OrderType.Buy, currentBar.Open, _volumeBase, magicNumber: _magicNumber);
                    //_trader.Trade(OrderType.Buy, currentBar.Open, volume);
                    Console.WriteLine($"Открываю позицию {OrderType.Buy.ToString()} с ema = {currentEma:F1}, дата {currentBar.Key}, цена {currentBar.Open}");
                }
            }
        }
    }
}
