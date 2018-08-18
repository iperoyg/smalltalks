using SmallTalks.Core.Models;

namespace SmallTalks.Core
{
    public interface ISmallTalksDetector
    {
        SmallTalksDectectorData DectectorData { get; }

        Analysis Detect(string input);
    }
}