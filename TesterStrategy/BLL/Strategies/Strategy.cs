using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Strategies
{
    public abstract class Strategy
    {
        // ReSharper disable once InconsistentNaming
        protected IChart _chart;
        
        public abstract void Configuration(Action<StrategyOption> config);

        public virtual void Run(IChart chart)
        {
            _chart = chart;
        }
    }
}
