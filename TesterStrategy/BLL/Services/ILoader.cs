using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TesterStrategy.Models;

namespace TesterStrategy.BLL.Services
{
    public interface ILoader
    {
        Task<Bar[]> LoadBars(string filename, CancellationToken token = default);
    }
}
