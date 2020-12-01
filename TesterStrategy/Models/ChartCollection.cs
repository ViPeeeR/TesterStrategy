using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.BLL;

namespace TesterStrategy.Models
{
    public class ChartCollection
    {
        /// <summary>
        /// График с барами М1
        /// </summary>
        public Chart Chart { get; }

        /// <summary>
        /// График с барами М5
        /// </summary>
        public Chart ChartM5 { get; set; }

        /// <summary>
        /// График с барами М15
        /// </summary>
        public Chart ChartM15 { get; set; }

        /// <summary>
        /// График с барами Н1
        /// </summary>
        public Chart ChartH1 { get; set; }

        /// <summary>
        /// График с барами Н4 ???
        /// </summary>
        public Chart ChartH4 { get; set; }

        /// <summary>
        /// График с барами Д1 (10.00 - 23.50)
        /// </summary>
        public Chart ChartD1 { get; set; }
    }
}
