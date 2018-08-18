using SmallTalks.Core.Models;

namespace SmallTalks.Core.Services
{
    public interface IDetectorDataProviderService
    {
        SmallTalksDectectorData GetSmallTalksDetectorDataFromSource(SourceProvider source);
    }
}