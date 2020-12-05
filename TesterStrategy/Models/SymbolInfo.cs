using System;
using TesterStrategy.BLL;
using TesterStrategy.BLL.Interfaces;

namespace TesterStrategy.Models
{
    public class SymbolInfo
    {
        public SymbolInfo(
            string name,
            double margin,
            double priceStep, 
            double pipsStep)
        {
            Margin = margin;
            PriceStep = priceStep;
            PipsStep = pipsStep;
            Id = name;
        }

        public string Id { get; }

        public double Margin { get; }

        public double PriceStep { get; }

        public double PipsStep { get; }
    }
}
