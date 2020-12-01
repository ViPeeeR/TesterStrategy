using System;
using TesterStrategy.BLL.Interfaces;

namespace TesterStrategy.BLL.Indicators
{
    public class MovingAverage
    {
        private readonly IChart _chart;
        private readonly int _period;

        public MovingAverage(
            IChart chart,
            int period)
        {
            _chart = chart;
            _period = period;
        }

        public double[] EmaValue()
        {
            var bars = _chart.GetBars();
            if (bars.Length <= _period + 1)
            {
                return Array.Empty<double>();
            }

            var pr = 2.0 / (_period + 1);
            var pos = bars.Length - 2;

            var buffer = new double[bars.Length];
            while (pos >= 0)
            {
                if (pos == bars.Length - 2)
                {
                    buffer[pos + 1] = bars[pos + 1].Close;
                }
                buffer[pos] = bars[pos].Close * pr + buffer[pos + 1] * (1 - pr);
                pos--;
            }

            return buffer;
        }
    }
}
