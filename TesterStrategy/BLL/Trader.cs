using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Trader : ITrader
    {
        private readonly ILogger<Trader> _logger;
        private readonly TraderOptions _traderOptions = new TraderOptions();
        private MarketInfo MarketInfo => _traderOptions.MarketInfo;

        public double Balance { get; private set; }

        public double Margin { get; private set; }

        public List<Deal> Orders { get; } = new List<Deal>();

        public List<Deal> History { get; } = new List<Deal>();

        public Trader(
            ILogger<Trader> logger)
        {
            _logger = logger;
        }

        public void Configuration(Action<TraderOptions> config)
        {
            config?.Invoke(_traderOptions);
            if (_traderOptions.Balance != null)
            {
                Balance = _traderOptions.Balance.Value;
            }

        }

        public void Trade(OrderType orderType, double price, int volume, int? takeprofitPips = null, int? stoplossPips = null)
        {
            var currentBar = MarketInfo.Chart.GetBar(0);
            if (!(price >= currentBar.Low && price <= currentBar.High))
            {
                // TODO: мы не можем открыть сделку по этой цене в этом баре
                return;
            }

            // TODO: сначала проверяем есть ли текущие сделки
            if (Orders.Any())
            {
                if (Orders.First().Type == orderType)
                {
                    // TODO: проверка, что по балансу мы можем открыть это количество сделок
                    if (Balance > 0 && MarketInfo.Margin * volume < Balance - Margin)
                    {
                        OpenOrder(orderType, currentBar.Key, price, volume, takeprofitPips, stoplossPips);
                    }
                }
                else
                {
                    // TODO: закрываем сделки и открываем новые, изучая маржу
                    var currentVolume = Orders.Sum(x => x.Volume);
                    if (currentVolume >= volume)
                    {
                        // TODO: закрываем часть сделки
                        CloseOrder(currentBar.Key, price, volume);
                    }
                    else // переворачиваемся
                    {
                        CloseOrder(currentBar.Key, price, currentVolume);
                        if (Balance > 0 && MarketInfo.Margin * (volume - currentVolume) < Balance - Margin)
                        {
                            OpenOrder(orderType, currentBar.Key, price, volume - currentVolume, takeprofitPips, stoplossPips);
                        }
                    }
                }
            }
            else
            {
                // TODO: проверка, что по балансу мы можем открыть это количество сделок
                if (Balance > 0 && MarketInfo.Margin * volume < Balance - Margin)
                {
                    OpenOrder(orderType, currentBar.Key, price, volume, takeprofitPips, stoplossPips);
                }
            }

            UpdateMargin();
        }

        private Guid OpenOrder(OrderType orderType, DateTime date, double price, int volume, int? takeprofitPips, int? stoplossPips)
        {
            var deal = new Deal
            {
                Id = Guid.NewGuid(),
                Type = orderType,
                OpenTime = date,
                PriceOpen = price,
                StopLoss = stoplossPips.HasValue
                    ? orderType == OrderType.Buy
                        ? price - stoplossPips
                        : price + stoplossPips
                    : null,
                TakeProfit = takeprofitPips.HasValue
                    ? orderType == OrderType.Buy
                        ? price + takeprofitPips
                        : price - takeprofitPips
                    : null,
                Volume = volume
            };

            _logger.LogInformation($"Открыта сделка {date} #{History.Count + 1} по цене {price}, объемом {volume} в сторону {orderType.ToString()}");

            Orders.Add(deal);
            return deal.Id;
        }

        private void CloseOrder(Guid id, DateTime date, double price)
        {
            var orderForClose = Orders.First(x => x.Id == id);
            var closeOrder = new Deal
            {
                Id = orderForClose.Id,
                OpenTime = orderForClose.OpenTime,
                PriceOpen = orderForClose.PriceOpen,
                StopLoss = orderForClose.StopLoss,
                TakeProfit = orderForClose.TakeProfit,
                Type = orderForClose.Type,
                Volume = orderForClose.Volume,
                CloseTime = date,
                PriceClose = price,
                Profit = orderForClose.Type == OrderType.Buy ? price - orderForClose.PriceOpen : orderForClose.PriceOpen - price
            };
            History.Add(closeOrder);
            Orders.Remove(orderForClose);
            UpdateBalance(closeOrder.Volume, closeOrder.Profit);
            UpdateMargin();

            _logger.LogInformation($"Закрыта сделка {date} #{History.Count} по цене {price}, объемом {closeOrder.Volume} с профитом {closeOrder.Profit} пипсов");
            _logger.LogDebug($"Текущий баланс составляет {Balance}");
        }

        private void UpdateBalance(int volume, double profit)
        {
            Balance += volume * MarketInfo.PriceStep * profit / MarketInfo.PipsStep;
        }

        private void UpdateMargin()
        {
            Margin = MarketInfo.Margin * Orders.Sum(x => x.Volume);
        }

        private void CloseOrder(DateTime date, double price, int volume)
        {
            var volumeLeft = volume;
            while (volumeLeft > 0)
            {
                var orderForClose = Orders.First();
                if (volumeLeft >= orderForClose.Volume)
                {
                    var closeOrder = new Deal
                    {
                        Id = orderForClose.Id,
                        OpenTime = orderForClose.OpenTime,
                        PriceOpen = orderForClose.PriceOpen,
                        StopLoss = orderForClose.StopLoss,
                        TakeProfit = orderForClose.TakeProfit,
                        Type = orderForClose.Type,
                        Volume = orderForClose.Volume,
                        CloseTime = date,
                        PriceClose = price,
                        Profit = orderForClose.Type == OrderType.Buy ? price - orderForClose.PriceOpen : orderForClose.PriceOpen - price
                    };
                    History.Add(closeOrder);
                    Orders.Remove(orderForClose);
                    volumeLeft -= orderForClose.Volume;
                    UpdateBalance(closeOrder.Volume, closeOrder.Profit);
                }
                else
                {
                    var closeOrder = new Deal
                    {
                        Id = orderForClose.Id,
                        OpenTime = orderForClose.OpenTime,
                        PriceOpen = orderForClose.PriceOpen,
                        StopLoss = orderForClose.StopLoss,
                        TakeProfit = orderForClose.TakeProfit,
                        Type = orderForClose.Type,
                        Volume = volumeLeft,
                        CloseTime = date,
                        PriceClose = price,
                        Profit = orderForClose.Type == OrderType.Buy ? price - orderForClose.PriceOpen : orderForClose.PriceOpen - price
                    };
                    History.Add(closeOrder);
                    orderForClose.Volume -= volumeLeft;
                    volumeLeft = 0;
                    UpdateBalance(closeOrder.Volume, closeOrder.Profit);
                }
            }
            UpdateMargin();
        }

        public void Update()
        {
            var currentBar = MarketInfo.Chart.GetBar(0);
            var needClose = new List<(Guid id, DateTime date, double price)>();
            foreach (var order in Orders)
            {
                if (order.StopLoss >= currentBar.Low && order.StopLoss <= currentBar.High
                    && order.TakeProfit >= currentBar.Low && order.TakeProfit <= currentBar.High)
                {
                    throw new Exception("Стоп лосс и тейкпрофит сработали одновременно");
                    // TODO: не понятно что сработало быстрее!! Надо как-то по особому это обработать
                    //needClose.Add((order.Id, currentBar.Key, order.StopLoss.Value));
                }

                // TODO: проверяем ТП и СЛ для сделок по тику.
                if (order.StopLoss.HasValue && order.StopLoss >= currentBar.Low && order.StopLoss <= currentBar.High)
                {
                    // TODO: закрываем сделку по стоплосу
                    needClose.Add((order.Id, currentBar.Key, order.StopLoss.Value));
                    continue;
                }

                if (order.TakeProfit.HasValue && order.TakeProfit >= currentBar.Low && order.TakeProfit <= currentBar.High)
                {
                    // TODO: закрываем сделку по тейкпрофиту
                    needClose.Add((order.Id, currentBar.Key, order.TakeProfit.Value));
                    continue;
                }
            }

            foreach (var (id, date, price) in needClose)
            {
                CloseOrder(id, date, price);
            }
        }
    }
}
