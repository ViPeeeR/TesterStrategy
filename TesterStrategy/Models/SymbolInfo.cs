using System;
using TesterStrategy.BLL;
using TesterStrategy.BLL.Interfaces;

namespace TesterStrategy.Models
{
    public class SymbolInfo
    {
        public SymbolInfo(
            string name,
            Bar[] bars, 
            double margin,
            double priceStep, 
            double pipsStep)
        {
            Chart = new Chart(bars);
            Margin = margin;
            PriceStep = priceStep;
            PipsStep = pipsStep;
            Id = name;
        }

        public string Id { get; set; }

        public IChart Chart { get; }

        public double Margin { get; }

        public double PriceStep { get; }

        public double PipsStep { get; }
    }
}
