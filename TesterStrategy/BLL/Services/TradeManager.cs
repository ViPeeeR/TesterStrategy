using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Exceptions;
using TesterStrategy.BLL.Interfaces;
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

        public double Balance => _startBalance +
                                 _symbol.PriceStep * _historyOrders.Sum(x => x.Volume * x.Profit) / _symbol.PipsStep;

        public double Margin => _symbol.Margin * _orders.Sum(x => x.Volume);

        private long _orderCounter = 0;

        public TradeManager(SymbolInfo symbol, double balance)
        {
            _symbol = symbol;
            _startBalance = balance;
            _orders = new List<Order>();
            _historyOrders = new List<Order>();
        }

        public void Open(IChart chart, OrderType orderType, double price, int volume, double? takeProfit = null,
            double? stopLoss = null, int magicNumber = 0)
        {
            if (!PriceIsAcceptableForCurrentBar(price, chart.Current))
            {
                throw new InvalidPriceException($"Недопустимая цена сделки {price:F2}.");
            }

            var currentBar = chart.Current;
            var openedOrder = _orders.FirstOrDefault();
            if (openedOrder != null && openedOrder.Type != orderType && openedOrder.MagicNumber == magicNumber)
            {
                throw new HaveOpenOrdersException("Имеются сделки в другую сторону. Для начала закройте их.");
            }

            OpenOrder(orderType, currentBar.Key, price, volume, takeProfit, stopLoss, magicNumber);
        }

        public void Close(IChart chart, double price, int? magicNumber)
        {
            if (!PriceIsAcceptableForCurrentBar(price, chart.Current))
            {
                throw new InvalidPriceException($"Недопустимая цена сделки {price:F2}.");
            }

            var currentBar = chart.Current;
            var orders = magicNumber.HasValue
                ? _orders.Where(x => x.MagicNumber == magicNumber).ToArray()
                : _orders.ToArray();
            if (!orders.Any())
            {
                throw new OrderNotFoundException("Нет сделок, чтобы их закрыть");
            }

            foreach (var order in orders)
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

                Console.WriteLine(
                    $"Закрыта сделка {order.Id}: CLOSE = {closingOrder.PriceClose}, DATE = {closingOrder.CloseTime}, PROFIT = {closingOrder.Profit}, BALANCE = {Balance}");
            }

            Array.ForEach(orders, x => _orders.Remove(x));
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

        public void UpdateOrders(IChart chart)
        {
            var currentBar = chart.Current;

            var expiredOrders = _orders
                .Select(order =>
                {
                    if (order.StopLoss >= currentBar.Low && order.StopLoss <= currentBar.High)
                    {
                        return (order.Id, order.StopLoss.Value);
                    }

                    if (order.TakeProfit >= currentBar.Low && order.TakeProfit <= currentBar.High)
                    {
                        return (order.Id, order.TakeProfit.Value);
                    }

                    return default;
                })
                .Where(x => x.Id != default)
                .ToArray();

            Array.ForEach(expiredOrders, order => CloseOrder(chart, order.Id, order.Value));
        }

        private void OpenOrder(OrderType orderType, DateTime date, double price, int volume, double? takeProfit,
            double? stopLoss, int magicNumber = 0)
        {
            if (CheckMarginForOpenPosition(volume))
            {
                var deal = new Order
                {
                    Id = ++_orderCounter,
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

        private void CloseOrder(IChart chart, long id, double price)
        {
            var currentBar = chart.Current;
            var order = _orders.Single(x => x.Id == id);

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
            _orders.Remove(order);
        }

        private bool CheckMarginForOpenPosition(int volume)
        {
            if (Balance > 0 && _symbol.Margin * volume < Balance - Margin)
            {
                return true;
            }

            throw new MarginFreeNotEnoughException("Нет свободного количества маржи для совершения сделки");
        }

        private bool PriceIsAcceptableForCurrentBar(double price, Bar currentBar)
        {
            return price >= currentBar.Low && price <= currentBar.High;
        }
    }
}