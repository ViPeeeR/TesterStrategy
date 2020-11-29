using System;
using System.Collections.Generic;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Interfaces
{
    public interface ITrader
    {
        void Configuration(Action<TraderOptions> config);

        void Trade(OrderType orderType, double price, int volume, int? takeprofitPips = null, int? stoplossPips = null);

        List<Deal> Orders { get; }

        List<Deal> History { get; }

        double Balance { get; }

        double Margin { get; }

        void Update();
    }
}