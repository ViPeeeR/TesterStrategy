using System;
using System.Collections.Generic;
using System.Text;

namespace TesterStrategy.Models
{
    public class Bar
    {
        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        /// <summary>
        /// Цена открытия свечи
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Максимальная цена свечи
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Минимальная цена свечи
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Цена закрытия свечи
        /// </summary>
        public double Close { get; set; }

        public long TickVolume { get; set; }

        public long Volume { get; set; }

        public double Spread { get; set; }

        public DateTime Key => Date.AddMinutes(Time.TotalMinutes);
    }
}
