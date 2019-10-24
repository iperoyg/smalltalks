using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    public class BatchAnalysisRequest
    {
        public string Id { get; set; }
        public List<ConfiguredAnalysisRequestItem> Items { get; set; }

        public BatchAnalysisRequest()
        {
            Items = new List<ConfiguredAnalysisRequestItem>();
        }

        public bool IsValid()
        {
            return Items != null && Items.Count > 0 && !string.IsNullOrEmpty(Id);
        }
    }
}
