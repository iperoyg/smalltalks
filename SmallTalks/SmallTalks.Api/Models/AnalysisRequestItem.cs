using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    public class AnalysisRequestItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool DateCheck { get; set; }

        public AnalysisRequestItem()
        {
            DateCheck = false;
        }
    }
}
