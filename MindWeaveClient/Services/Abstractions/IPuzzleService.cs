using MindWeaveClient.PuzzleManagerService;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IPuzzleService
    {
        /// <summary>
        /// Asynchronously retrieves the list of all available puzzles.
        /// </summary>
        /// <returns>An array of PuzzleInfoDto.</returns>
        Task<PuzzleInfoDto[]> getAvailablePuzzlesAsync();

        /// <summary>
        /// Asynchronously uploads a new custom puzzle image.
        /// </summary>
        /// <param name="username">The user uploading the image.</param>
        /// <param name="imageBytes">The raw byte array of the image file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>An UploadResultDto indicating success and the new puzzle ID.</returns>
        Task<UploadResultDto> uploadPuzzleImageAsync(string username, byte[] imageBytes, string fileName);
    }
}
