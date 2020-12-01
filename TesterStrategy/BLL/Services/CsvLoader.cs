using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TesterStrategy.BLL.Services.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services
{
    public class CsvLoader : ILoader
    {
        private readonly ILogger<CsvLoader> _logger;

        public CsvLoader(ILogger<CsvLoader> logger)
        {
            _logger = logger;
        }

        public async Task<Bar[]> LoadBars(string filename, CancellationToken token = default)
        {
            var lines = await ReadFile(filename, token);
            if (lines?.Any() != true)
            {
                return Array.Empty<Bar>();
            }

            var bars = new List<Bar>();
            foreach (var item in lines)
            {
                var values = item.Split(";");
                if (item.Trim().Length == 0 || values?.Any() != true || values[0].IndexOf("<") != -1)
                {
                    continue;
                }

                bars.Add(new Bar
                {
                    Date = DateTime.Parse(values[0]),
                    Time = TimeSpan.Parse(values[1]),
                    Open = double.Parse(values[2]),
                    High = double.Parse(values[3]),
                    Low = double.Parse(values[4]),
                    Close = double.Parse(values[5]),
                    TickVolume = long.Parse(values[6]),
                    Volume = long.Parse(values[7]),
                    Spread = double.Parse(values[8]),
                });
            }

            _logger.LogInformation($"File was load and contains {bars.Count} bars");

            return bars.ToArray();
        }

        private async Task<string[]> ReadFile(string filename, CancellationToken token)
        {
            try
            {
                using var fileStream = new FileStream(filename, FileMode.Open);
                var buffer = new byte[fileStream.Length];
                var readingBytes = await fileStream.ReadAsync(buffer, 0, (int)fileStream.Length, token);
                return Encoding.UTF8.GetString(buffer.ToArray())?.Split("\r\n");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error read file {filename}", ex);
                return Array.Empty<string>();
            }
        }
    }
}
