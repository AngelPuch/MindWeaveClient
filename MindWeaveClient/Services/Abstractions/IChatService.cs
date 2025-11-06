using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IChatService
    {
        bool connect(string username, string lobbyId);
        void disconnect();
        Task sendLobbyMessageAsync(string username, string lobbyId, string message);
        bool isConnected();
    }
}
