using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    public class BatchAnalysisResponse
    {
        public string Id { get; set; }
        public List<AnalysisResponseItem> Items { get; set; }

        public BatchAnalysisResponse()
        {
            Id = string.Empty;
            Items = new List<AnalysisResponseItem>();
            
        }
    }
}
