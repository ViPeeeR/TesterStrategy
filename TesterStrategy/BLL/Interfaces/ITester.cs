using System.Threading;
using System.Threading.Tasks;

namespace TesterStrategy.BLL.Interfaces
{
    public interface ITester
    {
        Task Run(CancellationToken token = default);
    }
}
