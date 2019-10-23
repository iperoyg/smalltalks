using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    [DataContract]
    public class AnalysisRequestItem
    {
        [DataMember(Name ="id")]
        public string Id { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "dateCheck")]
        public bool CheckDate { get; set; }

        public AnalysisRequestItem()
        {
            CheckDate = false;
        }
    }
}
