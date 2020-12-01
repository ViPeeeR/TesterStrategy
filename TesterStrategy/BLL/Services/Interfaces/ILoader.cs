using System.Threading;
using System.Threading.Tasks;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services.Interfaces
{
    public interface ILoader
    {
        Task<Bar[]> LoadBars(string filename, CancellationToken token = default);
    }
}
