using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services
{
    public class ChartManager : IChartManager
    {
        private readonly Bar[] _allBars;
        private readonly List<Bar> _currentChart = new List<Bar>();

        public ChartManager(Bar[] bars)
        {
            _allBars = bars ?? throw new ArgumentNullException(nameof(bars));
        }

        public IChart Chart { get; private set; }

        public bool Next()
        {
            var currentIndex = 0;
            if (_currentChart.Any())
            {
                var currentDate = _currentChart[0].Key;
                currentIndex = _allBars
                    .Select((value, index) => (Date: value.Key, Index: index))
                    .First(x => x.Date == currentDate)
                    .Index + 1;
            }

            if (currentIndex >= _allBars.Length)
            {
                return false;
            }

            _currentChart.Insert(0, _allBars[currentIndex]);
            Chart = new Chart(_currentChart);
            
            return true;
        }

        public void Finish()
        {
            _currentChart.Clear();
            _currentChart.AddRange(_allBars.Reverse());
            Chart = new Chart(_currentChart);
        }
    }
}