using DateTimeDectector.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    [DataContract]
    public class AnalysisResponseItem
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name = "analysis")]
        public Core.Models.Analysis SmallTalksAnalysis { get; set; }

        [DataMember(Name = "datetimes")]
        public List<DateTimeDectected> DateTimeDectecteds { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
