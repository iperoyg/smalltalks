using System.Threading.Tasks;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface IWordsDetector
    {
        Task LoadAsync(string wordFile);
        Task<bool> HaveWordsAsync(string input);
        Task<string> RemoveWordsAsync(string input);
        Task<(string, bool)> ReplaceWordsAsync(string input, char placeholder);
    }
}