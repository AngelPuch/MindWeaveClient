using System.Threading.Tasks;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface ISessionCleanupService
    {
        Task cleanUpSessionAsync();
        Task exitGameInProcessAsync();
        Task exitLobbyAsync(string lobbyCode);
    }
}