using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Strategies
{
    public interface IStrategy
    {
        void Configuration(Action<StrategyOption> config);

        bool Run();
    }
}
