using SmallTalks.Core.Models;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface IDetectorDataProviderService
    {
        Task<SmallTalksDectectorData> GetSmallTalksDetectorDataFromSourceAsync(SourceProvider source);

        Task<SmallTalksDectectorData> GetSmallTalksDetectorDataFromSourceAsyncv2(SourceProvider source);
    }
}