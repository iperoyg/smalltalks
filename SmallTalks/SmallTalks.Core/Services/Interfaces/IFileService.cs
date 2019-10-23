using SmallTalks.Core.Models;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface IFileService
    {
        Task<RulesData> ReadRulesFromSourceAsync(string file);
    }
}