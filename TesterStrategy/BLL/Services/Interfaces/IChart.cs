using System;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services.Interfaces
{
    public interface IChart
    {
        Bar Current { get; }

        Bar GetBar(DateTime key);

        Bar GetBar(int index);

        Bar[] GetBars();
    }
}
