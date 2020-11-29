using System;
using System.Collections.Generic;
using System.Linq;

namespace TesterStrategy.Models
{
    public class Chart
    {
        private readonly Bar[] _allBars;
        private readonly List<Bar> _currentChart = new List<Bar>();

        public Chart(Bar[] history)
        {
            _allBars = history;
        }

        public Bar GetBar(DateTime key)
        {
            return _currentChart.FirstOrDefault(x => x.Key == key);
        }

        public Bar GetBar(int index)
        {
            if (_currentChart.Any())
            {
                return _currentChart[0];
            }

            return null;
        }

        public Bar Next()
        {
            int index = 0;
            if (_currentChart.Any())
            {
                var currentKey = _currentChart[0].Key;
                index = _allBars
                    .Select((value, index) => (value, index))
                    .First(x => x.value.Key == currentKey)
                    .index + 1;
            }

            if (index >= _allBars.Length)
            {
                return null;
            }

            _currentChart.Insert(0, _allBars[index]);
            return _currentChart[0];
        }
    }
}
