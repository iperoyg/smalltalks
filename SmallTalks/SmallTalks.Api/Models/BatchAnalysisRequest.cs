using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    public class BatchAnalysisRequest
    {
        public string Id { get; set; }
        public List<AnalysisRequestItem> Items { get; set; }

        public BatchAnalysisRequest()
        {
            Items = new List<AnalysisRequestItem>();
        }
    }
}
