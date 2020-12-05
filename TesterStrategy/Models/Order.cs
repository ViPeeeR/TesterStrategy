using System;
using System.Collections.Generic;
using System.Text;

namespace TesterStrategy.Models
{
    public class Order
    {
        public long Id { get; set; }

        /// <summary>
        /// Цена открытия сделки
        /// </summary>
        public double PriceOpen { get; set; }

        /// <summary>
        /// Время открытия сделки
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Тип сделки
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Объем в сделке
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Цена закрытия сделки
        /// </summary>
        public double PriceClose { get; set; }
        
        /// <summary>
        /// Время закрытия сделки
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// Цена для исполнения по тейк профиту
        /// </summary>
        public double? TakeProfit { get; set; }

        /// <summary>
        /// Цена для исполнения по стоп лосу
        /// </summary>
        public double? StopLoss { get; set; }

        /// <summary>
        /// Полученная прибыль или убыток
        /// </summary>
        public double Profit { get; set; }

        /// <summary>
        /// Номер системы, которая следит за ордером
        /// </summary>
        public int MagicNumber { get; set; }

        public string Symbol { get; set; }

    }
}
