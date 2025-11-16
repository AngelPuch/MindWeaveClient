using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services.Abstractions;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class PuzzleService : IPuzzleService
    {
        public async Task<PuzzleInfoDto[]> getAvailablePuzzlesAsync()
        {
            PuzzleManagerClient client = new PuzzleManagerClient();
            try
            {
                PuzzleInfoDto[] result = await client.getAvailablePuzzlesAsync();
                client.Close();
                return result;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<UploadResultDto> uploadPuzzleImageAsync(string username, byte[] imageBytes, string fileName)
        {
            PuzzleManagerClient client = new PuzzleManagerClient();
            try
            {
                UploadResultDto result = await client.uploadPuzzleImageAsync(username, imageBytes, fileName);
                client.Close();
                return result;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<PuzzleDefinitionDto> getPuzzleDefinitionAsync(int puzzleId, int difficultyId)
        {
            PuzzleManagerClient client = new PuzzleManagerClient();
            try
            {
                PuzzleDefinitionDto result = await client.getPuzzleDefinitionAsync(puzzleId, difficultyId);
                client.Close();
                return result;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }
    }
}
