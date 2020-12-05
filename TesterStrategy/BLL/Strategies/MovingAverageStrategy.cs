using System;
using System.Linq;
using TesterStrategy.BLL.Indicators;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Strategies
{
    public class MovingAverageStrategy : Strategy
    {
        private readonly int _period;
        private ITradeManager _tradeManager;

        private int _volumeBase = 1;
        private readonly int _magicNumber;

        public MovingAverageStrategy(int period)
        {
            _period = period;
            _magicNumber = new Random().Next(100, 99999);
        }

        public override void Configuration(Action<StrategyOption> config)
        {
            var options = new StrategyOption();
            config?.Invoke(options);
            _tradeManager = options.TradeManager ?? throw new ArgumentNullException(nameof(options.TradeManager));
            _volumeBase = options.Volume ?? 1;
        }

        public override void Run(IChart chart)
        {
            base.Run(chart);
            
            var movingAverage = new MovingAverage(chart, _period);
            
            var ema = movingAverage.EmaValue();
            if (ema.Length == 0)
                return;

            if (_tradeManager.GetOrders(_magicNumber).Any())
            {
                ClosePosition(ema);
            }

            if (!_tradeManager.GetOrders(_magicNumber).Any())
            {
                OpenPosition(ema);
            }
        }

        private void ClosePosition(double[] ema)
        {
            var currentBar = _chart.Current;
            var position = _tradeManager.GetOrders(_magicNumber).Select(x => x.Type).Single();
            switch (position)
            {
                case OrderType.Buy when SellSignal(ema):
                case OrderType.Sell when BuySignal(ema):
                    _tradeManager.Close(_chart, currentBar.Open, _magicNumber);
                    break;
            }
        }

        private void OpenPosition(double[] ema)
        {
            var currentBar = _chart.Current;
            if (SellSignal(ema))
            {
                _tradeManager.Open(_chart, OrderType.Sell, currentBar.Open, _volumeBase, magicNumber: _magicNumber);
                Console.WriteLine($"Сигнал на {OrderType.Sell.ToString()}: OPEN = {currentBar.Open}, DATE = {currentBar.Key}, EMA = {ema[1]:F1}");
            }
            else
            {
                if (BuySignal(ema))
                {
                    _tradeManager.Open(_chart, OrderType.Buy, currentBar.Open, _volumeBase, magicNumber: _magicNumber);
                    Console.WriteLine($"Сигнал на {OrderType.Buy.ToString()}: OPEN = {currentBar.Open}, DATE = {currentBar.Key}, EMA = {ema[1]:F1}");
                }
            }
        }
        
        private bool BuySignal(double[] ema)
        {
            var lastEma = ema[1];
            var lastBar = _chart.GetBar(1);
            return lastBar.Open < lastEma && lastBar.Close > lastEma;
        }

        private bool SellSignal(double[] ema)
        {
            var lastEma = ema[1];
            var lastBar = _chart.GetBar(1);
            return lastBar.Open > lastEma && lastBar.Close < lastEma;
        }
    }
}
