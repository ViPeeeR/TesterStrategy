using System;
using System.Collections.Generic;
using System.Text;

namespace TesterStrategy.Models
{
    public class SymbolOptions
    {
        public string Name { get; set; }

        public Bar[] Bars { get; set; }

        /// <summary>
        /// Маржа на один контракт
        /// </summary>
        public double Margin { get; set; }

        /// <summary>
        /// Стоимость шага цены
        /// </summary>
        public double PriceStep { get; set; }

        /// <summary>
        /// Шаг цены в пипсах
        /// </summary>
        public double PipsStep { get; set; }
    }
}
