using DateTimeDectector.Domain;
using Newtonsoft.Json;
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


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
