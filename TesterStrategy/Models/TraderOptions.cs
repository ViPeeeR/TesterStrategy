using System;
using System.Collections.Generic;
using System.Text;
using TesterStrategy.BLL.Strategies;

namespace TesterStrategy.Models
{
    public class TraderOptions
    {
        public double? Balance { get; set; }

        public Strategy[] Strategies { get; set; }
    }
}
