using SmallTalks.Core.Models;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface ISourceProviderService
    {
        SourceProvider GetSourceProvider();

        SourceProvider GetSourceProviderv2();
    }
}