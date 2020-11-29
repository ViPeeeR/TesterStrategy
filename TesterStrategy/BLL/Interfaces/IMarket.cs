using System;
using System.Collections.Generic;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Interfaces
{
    public interface IMarket
    {
        void Configuration(Action<MarketOptions> options);

        bool Tick();

        MarketInfo MarketInfo { get; }
    }
}