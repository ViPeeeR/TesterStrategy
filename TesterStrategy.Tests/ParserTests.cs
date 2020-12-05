using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesterStrategy.BLL;
using TesterStrategy.BLL.Helpers;
using TesterStrategy.BLL.Indicators;
using TesterStrategy.BLL.Services;
using TesterStrategy.Models;
using Xunit;

namespace TesterStrategy.Tests
{
    public class ParserTests
    {
        [Fact]
        public async Task ReadFile_FileShouldRead_Success()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CsvLoader>>();
            var parser = new CsvLoader(loggerMock);
            string file = Path.Combine(Directory.GetCurrentDirectory(), "RTS-9.20_M1.csv");
            string fileH1 = Path.Combine(Directory.GetCurrentDirectory(), "RTS-9.20_H1.csv");

            // Act
            var bars = await parser.LoadBars(fileH1);
            //var barsH1Original = await parser.LoadBars(fileH1);

            //var barsM5 = ChartHelper.ConvertToPeriod(bars, 5);
            //var barsM15 = ChartHelper.ConvertToPeriod(bars, 15);
            //var barsH1 = ChartHelper.ConvertToPeriod(bars, PeriodType.H1);

            //var except = barsH1Original.Where(x => barsH1.All(b => b.Key != x.Key)).ToArray();

            // Assert
            bars.Should().NotBeEmpty();
            bars.Should().HaveCount(863);
        }

        [Fact]
        public async Task Ema_MA_Success()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CsvLoader>>();
            var parser = new CsvLoader(loggerMock);
            var fileH1 = Path.Combine(Directory.GetCurrentDirectory(), "RTS-12.20_H1.csv");
            var bars = await parser.LoadBars(fileH1);
            var chartManager = new ChartManager(bars);
            chartManager.Finish();
            var ema = new MovingAverage(chartManager.Chart, 50);

            // Act
            var emaValues = ema.EmaValue();


            // Assert
            bars.Should().NotBeEmpty();
            bars.Should().HaveCount(863);
        }
    }
}
