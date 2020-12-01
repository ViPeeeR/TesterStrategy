using System;
using System.Collections.Generic;
using System.Linq;
using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL
{
    public class Chart : IChart
    {
        private readonly Bar[] _allBars;
        private readonly List<Bar> _currentChart = new List<Bar>();

        public Bar Current => GetBar(0);

        public Chart(Bar[] bars)
        {
            _allBars = bars ?? throw  new ArgumentNullException(nameof(bars));
        }

        public Bar GetBar(DateTime key)
        {
            return _currentChart.FirstOrDefault(x => x.Key == key);
        }

        public Bar GetBar(int index)
        {
            if (_currentChart.Any())
            {
                return _currentChart[index];
            }

            return null;
        }

        public Bar[] GetBars()
        {
            return _currentChart.ToArray();
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

        public void Finish()
        {
            _currentChart.Clear();
            _currentChart.AddRange(_allBars.Reverse());
        }
    }
}
