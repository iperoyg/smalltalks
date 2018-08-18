using SmallTalks.Core.Models;

namespace SmallTalks.Core.Services
{
    public interface IConversionService
    {
        SmallTalksDectectorData ToDetectorData(RulesData rules);
        SmallTalksIntent ToIntent(Rule rule);
    }
}