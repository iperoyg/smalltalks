using SmallTalks.Core.Models;

namespace SmallTalks.Core.Services
{
    public interface IFileService
    {
        RulesData ReadRulesFromFile(string file);
    }
}