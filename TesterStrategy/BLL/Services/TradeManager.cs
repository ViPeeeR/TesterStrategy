using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services
{
    public class TradeManager : ITradeManager
    {
        private readonly SymbolInfo _symbol;

        private readonly ICollection<Order> _orders;
        private readonly ICollection<Order> _historyOrders;
        private readonly double _startBalance;
        
        public double Balance => _startBalance + _symbol.PriceStep * _historyOrders.Sum(x => x.Volume * x.Profit) / _symbol.PipsStep;

        public double Margin => _symbol.Margin * _orders.Sum(x => x.Volume);

        public TradeManager(SymbolInfo symbol, double balance)
        {
            _symbol = symbol;
            _startBalance = balance;
            _orders = new List<Order>();
            _historyOrders = new List<Order>();
        }

        public void Open(OrderType orderType, double price, int volume, double? takeProfit = null, double? stopLoss = null, int magicNumber = 0)
        {
            if (!PriceIsAcceptableForCurrentBar(price))
            {
                throw new Exception($"Недопустимая цена сделки {price:F2}.");
            }

            var currentBar = _symbol.Chart.Current;

            if (_orders.Any() && _orders.First().Type != orderType)
            {
                throw new Exception("Имеются сделки в другую сторону. Для начала закройте их.");
            }

            OpenOrder(orderType, currentBar.Key, price, volume, takeProfit, stopLoss);
        }

        public void Close(double price)
        {
            if (!PriceIsAcceptableForCurrentBar(price))
            {
                throw new Exception($"Недопустимая цена сделки {price:F2}.");
            }

            var currentBar = _symbol.Chart.Current;

            if (!_orders.Any())
            {
                throw new Exception("Нет сделок, чтобы их закрыть");
            }

            foreach (var order in _orders)
            {
                var closingOrder = new Order
                {
                    Id = order.Id,
                    OpenTime = order.OpenTime,
                    PriceOpen = order.PriceOpen,
                    StopLoss = order.StopLoss,
                    TakeProfit = order.TakeProfit,
                    Type = order.Type,
                    Volume = order.Volume,
                    CloseTime = currentBar.Key,
                    PriceClose = price,
                    Profit = order.Type == OrderType.Buy ? price - order.PriceOpen : order.PriceOpen - price
                };
                _historyOrders.Add(closingOrder);
            }
            _orders.Clear();
        }

        public IReadOnlyList<Order> GetOrders(int magicNumber)
        {
            return _orders
                .Where(x => x.MagicNumber == magicNumber)
                .ToArray();
        }

        public IReadOnlyList<Order> GetOrders()
        {
            return _orders.ToArray();
        }

        public IReadOnlyList<Order> GetHistory()
        {
            return _historyOrders.ToArray();
        }

        public void Update()
        {
            throw new NotImplementedException();

            //var currentBar = symbolInfo.Chart.GetBar(0);
            //var needClose = new List<(Guid id, DateTime date, double price)>();
            //foreach (var order in Orders)
            //{
            //    if (order.StopLoss >= currentBar.Low && order.StopLoss <= currentBar.High
            //        && order.TakeProfit >= currentBar.Low && order.TakeProfit <= currentBar.High)
            //    {
            //        throw new Exception("Стоп лосс и тейкпрофит сработали одновременно");
            //        // TODO: не понятно что сработало быстрее!! Надо как-то по особому это обработать
            //        //needClose.Add((order.Id, currentBar.Key, order.StopLoss.Value));
            //    }

            //    // TODO: проверяем ТП и СЛ для сделок по тику.
            //    if (order.StopLoss.HasValue && order.StopLoss >= currentBar.Low && order.StopLoss <= currentBar.High)
            //    {
            //        // TODO: закрываем сделку по стоплосу
            //        needClose.Add((order.Id, currentBar.Key, order.StopLoss.Value));
            //        continue;
            //    }

            //    if (order.TakeProfit.HasValue && order.TakeProfit >= currentBar.Low && order.TakeProfit <= currentBar.High)
            //    {
            //        // TODO: закрываем сделку по тейкпрофиту
            //        needClose.Add((order.Id, currentBar.Key, order.TakeProfit.Value));
            //        continue;
            //    }
            //}

            //foreach (var (id, date, price) in needClose)
            //{
            //    CloseOrder(id, date, price);
            //}
        }

        private void OpenOrder(OrderType orderType, DateTime date, double price, int volume, double? takeProfit,
            double? stopLoss, int magicNumber = 0)
        {
            if (CheckMarginForOpenPosition(volume))
            {
                var deal = new Order
                {
                    Id = Guid.NewGuid(),
                    Type = orderType,
                    OpenTime = date,
                    PriceOpen = price,
                    StopLoss = stopLoss,
                    TakeProfit = takeProfit,
                    Volume = volume,
                    MagicNumber = magicNumber,
                    Symbol = _symbol.Id
                };

                _orders.Add(deal);
            }
        }

        private bool CheckMarginForOpenPosition(int volume)
        {
            if (Balance > 0 && _symbol.Margin * volume < Balance - Margin)
            {
                throw new Exception("Нет свободного количества маржи для этой сделки");
                //_logger.LogWarning($"Не хватает маржи, чтобы открыть сделку  startBalance = {_startBalance}, _margin = {_margin}");
            }

            return true;
        }

        private bool PriceIsAcceptableForCurrentBar(double price)
        {
            var currentBar = _symbol.Chart.Current;
            return price >= currentBar.Low && price <= currentBar.High;
        }
    }
}
