using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class PuzzleService : IPuzzleService
    {
        public async Task<PuzzleInfoDto[]> getAvailablePuzzlesAsync()
        {
            return await executeSafeAsync(async (client) =>
                await client.getAvailablePuzzlesAsync());
        }

        public async Task<UploadResultDto> uploadPuzzleImageAsync(string username, byte[] imageBytes, string fileName)
        {
            return await executeSafeAsync(async (client) =>
                await client.uploadPuzzleImageAsync(username, imageBytes, fileName));
        }

        public async Task<PuzzleDefinitionDto> getPuzzleDefinitionAsync(int puzzleId, int difficultyId)
        {
            return await executeSafeAsync(async (client) =>
                await client.getPuzzleDefinitionAsync(puzzleId, difficultyId));
        }

        private async Task<T> executeSafeAsync<T>(Func<PuzzleManagerClient, Task<T>> action)
        {
            var client = new PuzzleManagerClient();
            try
            {
                T result = await action(client);
                client.Close();
                return result;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }
    }
}
