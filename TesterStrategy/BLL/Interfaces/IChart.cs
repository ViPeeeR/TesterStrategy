using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Interfaces
{
    public interface IChart
    {
        Bar Current { get; }

        Bar GetBar(DateTime key);

        Bar GetBar(int index);

        Bar[] GetBars();

        Bar Next();

        void Finish();
    }
}
