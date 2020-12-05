using TesterStrategy.BLL.Interfaces;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services.Interfaces
{
    public interface IChartManager
    {
        IChart Chart { get; }
        
        bool Next();

        void Finish();
    }
}