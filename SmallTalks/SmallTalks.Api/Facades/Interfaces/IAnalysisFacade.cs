using SmallTalks.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Facades.Interfaces
{
    public interface IAnalysisFacade
    {
        Task<AnalysisResponseItem> AnalyseAsync(string text, bool checkDate = true, int infoLevel = 1);
        Task<AnalysisResponseItem> ConfiguredAnalyseAsync(ConfiguredAnalysisRequestItem requestItem);
        Task<BatchAnalysisResponse> BatchAnalyse(BatchAnalysisRequest request);
    }
}
