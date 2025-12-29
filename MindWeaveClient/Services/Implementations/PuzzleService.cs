using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class PuzzleService : IPuzzleService
    {
        public async Task<PuzzleInfoDto[]> getAvailablePuzzlesAsync()
        {
            return await executeServiceCallAsync(async (client) =>
                await client.getAvailablePuzzlesAsync());
        }

        public async Task<UploadResultDto> uploadPuzzleImageAsync(string username, byte[] imageBytes, string fileName)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.uploadPuzzleImageAsync(username, imageBytes, fileName));
        }

        public async Task<PuzzleDefinitionDto> getPuzzleDefinitionAsync(int puzzleId, int difficultyId)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.getPuzzleDefinitionAsync(puzzleId, difficultyId));
        }

        private static async Task<T> executeServiceCallAsync<T>(Func<PuzzleManagerClient, Task<T>> action)
        {
            var client = new PuzzleManagerClient();
            try
            {
                T result = await action(client);
                closeClientSafe(client);
                return result;
            }
            catch (EndpointNotFoundException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (CommunicationException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (TimeoutException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (SocketException)
            {
                abortClientSafe(client);
                throw;
            }
        }

        private static void closeClientSafe(PuzzleManagerClient client)
        {
            try
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
        }

        private static void abortClientSafe(PuzzleManagerClient client)
        {
            try
            {
                client.Abort();
            }
            catch
            {
                // Ignore
            }
        }

    }
}
