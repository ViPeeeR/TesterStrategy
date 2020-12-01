using System;
using System.Collections.Generic;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Interfaces
{
    public interface ITrader
    {
        void Configuration(Action<TraderOptions> config);

        IReadOnlyList<Order> Orders { get; }

        IReadOnlyList<Order> History { get; }

        double Balance { get; }

        double Margin { get; }

        void Update();
    }
}