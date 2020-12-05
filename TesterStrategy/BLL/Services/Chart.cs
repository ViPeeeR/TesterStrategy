using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services
{
    public class Chart : IChart
    {
        private readonly IReadOnlyList<Bar> _bars;

        public Bar Current => GetBar(0);

        public Chart(IReadOnlyList<Bar> bars)
        {
            _bars = bars ?? throw new ArgumentNullException(nameof(bars));
        }

        public Bar GetBar(DateTime key)
        {
            return _bars.FirstOrDefault(x => x.Key == key);
        }

        public Bar GetBar(int index)
        {
            return _bars.Any() ? _bars[index] : null;
        }

        public Bar[] GetBars()
        {
            return _bars.ToArray();
        }
    }
}