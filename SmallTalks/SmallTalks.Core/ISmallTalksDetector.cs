using SmallTalks.Core.Models;
using System.Threading.Tasks;

namespace SmallTalks.Core
{
    public interface ISmallTalksDetector
    {
        SmallTalksDectectorData DectectorData { get; }

        Task<Analysis> DetectAsync(string input);
        Task<Analysis> DetectAsync(string input, SmallTalksPreProcessingConfiguration configuration);
    }
}