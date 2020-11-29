using System;
using System.Collections.Generic;
using System.Text;

namespace TesterStrategy.Models
{
    public class MarketInfo
    {
        public MarketInfo(Chart chart, double margin,
            double priceStep, double pipsStep)
        {
            Chart = chart;
            Margin = margin;
            PriceStep = priceStep;
            PipsStep = pipsStep;
        }

        public Chart Chart { get; }

        public double Margin { get; }

        public double PriceStep { get; }

        public double PipsStep { get; }
    }
}
