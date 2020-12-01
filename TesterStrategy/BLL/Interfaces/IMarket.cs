using System;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Interfaces
{
    public interface IMarket
    {
        void SetSymbol(Action<SymbolOptions> config);

        bool Tick();
    }
}