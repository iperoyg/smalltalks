using SmallTalks.Core.Models;
using System.Threading.Tasks;

namespace SmallTalks.Core
{
    public interface ISmallTalksDetector
    {
        SmallTalksDectectorData DetectorData { get; }

        SmallTalksDectectorData DetectorDatav2 { get; }

        Task<Analysis> DetectAsync(string input);

        Task<Analysis> DetectAsync(string input, SmallTalksPreProcessingConfiguration configuration);

        Task<Analysis> DetectAsyncv2(string input);

        Task<Analysis> DetectAsyncv2(string input, SmallTalksPreProcessingConfiguration configuration);
    }
}