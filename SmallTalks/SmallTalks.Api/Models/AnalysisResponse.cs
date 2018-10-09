using DateTimeDectector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    public class AnalysisResponse
    {
        public Core.Models.Analysis SmallTalksAnalysis { get; set; }
        public List<DateTimeDectected> DateTimeDectecteds { get; set; }

    }
}
