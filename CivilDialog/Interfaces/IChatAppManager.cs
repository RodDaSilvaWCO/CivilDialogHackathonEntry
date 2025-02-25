using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog.Interfaces
{
    public interface IChatAppManager : IDisposable
    {
        Task<string> RunInferenceAsync(string prompt);
        Task<bool> CreateInstanceAsync();
        void ShutDownInstance();

        bool IsInitialized { get; }
    }
}
