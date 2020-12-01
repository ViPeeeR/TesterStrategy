using System;
using System.Collections.Generic;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services.Interfaces
{
    public interface ITradeManager
    {
        void Open(OrderType orderType, double price, int volume, double? takeProfit = null, double? stopLoss = null, int magicNumber = 0);

        void Close(double price);

        IReadOnlyList<Order> GetOrders(int magicNumber);

        IReadOnlyList<Order> GetOrders();

        IReadOnlyList<Order> GetHistory();

        double Balance { get; }

        double Margin { get; }

        void Update();
    }
}
