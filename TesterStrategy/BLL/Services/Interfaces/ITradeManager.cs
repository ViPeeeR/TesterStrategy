using System;
using System.Collections.Generic;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services.Interfaces
{
    public interface ITradeManager
    {
        void Open(IChart chart, OrderType orderType, double price, int volume, double? takeProfit = null, double? stopLoss = null, int magicNumber = 0);

        void Close(IChart chart, double price, int? magicNumber);

        IReadOnlyList<Order> GetOrders(int magicNumber);

        IReadOnlyList<Order> GetOrders();

        IReadOnlyList<Order> GetHistory();

        double Balance { get; }

        double Margin { get; }

        void UpdateOrders(IChart chart);
    }
}
