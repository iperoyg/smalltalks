using SmallTalks.Core.Models;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services
{
    public interface IDetectorDataProviderService
    {
        Task<SmallTalksDectectorData> GetSmallTalksDetectorDataFromSourceAsync(SourceProvider source);
    }
}