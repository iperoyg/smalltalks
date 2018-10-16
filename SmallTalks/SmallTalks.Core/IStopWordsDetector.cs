using System.Threading.Tasks;

namespace SmallTalks.Core
{
    public interface IStopWordsDetector
    {
        Task<bool> HaveStopWordsAsync(string input);
        Task<string> RemoveStopWordsAsync(string input);
    }
}