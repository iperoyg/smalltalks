using SmallTalks.Core.Models;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface IConversionService
    {
        SmallTalksDectectorData ToDetectorData(RulesData rules);

        SmallTalksDectectorData ToDetectorDatav2(RulesData rules);

        SmallTalksIntent ToIntent(Rule rule);

        SmallTalksIntent ToIntentv2(Rule rule);
    }
}