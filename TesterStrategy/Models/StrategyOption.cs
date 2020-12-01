using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.BLL.Services.Interfaces;

namespace TesterStrategy.Models
{
    public class StrategyOption
    {
        public int? Volume { get; set; }

        public ITradeManager TradeManager { get; set; }

        public SymbolInfo Symbol { get; set; }
    }
}
