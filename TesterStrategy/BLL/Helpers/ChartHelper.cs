using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Helpers
{
    public static class ChartHelper
    {
        public static IEnumerable<Bar> ConvertToPeriod(Bar[] barsM1, PeriodType period)
        {
            var result = new List<Bar>();
            int took = 0;
            while (took < barsM1.Length)
            {
                var take = TakeBars(barsM1, period, took);
                var time = GetTime(period, take);

                var bar = new Bar
                {
                    Date = take.First().Date,
                    Time = GetTime(period, take),
                    Close = take.Last().Close,
                    High = take.Max(x => x.High),
                    Low = take.Max(x => x.Low),
                    Open = take.First().Open,
                    Spread = take.First().Spread,
                    TickVolume = take.Sum(x => x.TickVolume),
                    Volume = take.Sum(x => x.Volume)
                };
                result.Add(bar);
                took += take.Count();
            }

            return result;
        }

        private static TimeSpan GetTime(PeriodType period, IEnumerable<Bar> take)
        {
            switch (period)
            {
                case PeriodType.H1:
                    return TimeSpan.FromHours(take.First().Time.Hours);

                default: throw new NotImplementedException();
            }
            //return period == PeriodType.H1 ? TimeSpan.FromHours(take.First().Time.Hours) : take.First().Time;
        }

        private static IEnumerable<Bar> TakeBars(Bar[] bars, PeriodType period, int skip)
        {
            var result = new List<Bar>();
            foreach (var (bar, index) in bars.Select((bar, index) => (bar, index)).Skip(skip))
            {
                result.Add(bar);
                if (IsBreak(bars, index, period))
                    break;
            }
            return result;
        }


        private static bool IsBreak(Bar[] bars, int index, PeriodType period)
        {
            switch (period)
            {
                //case 5:
                //case 15:
                //    if ((bars[index].Key.Minute + 1) % period == 0)
                //        return true;
                //    return false;
                case PeriodType.H1:
                    if (bars.Length > (index + 1) && (bars[index].Key.Minute > bars[index + 1].Key.Minute))
                        return true;
                    return false;

                default: throw new NotImplementedException();
            }
        }
    }
}
