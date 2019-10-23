using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SmallTalks.Api.Models
{
    [DataContract]
    public class ConfiguredAnalysisRequestItem : AnalysisRequestItem
    {
        [DataMember(Name = "configuration")]
        public SmallTalksPreProcessingConfiguration Configuration { get; set; }
    }
}
