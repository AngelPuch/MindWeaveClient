using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IHeartbeatService : IDisposable
    {
        event Action<string> OnConnectionTerminated;

        Task<bool> startAsync(string username);

        Task stopAsync();

        void forceStop();
    }
}