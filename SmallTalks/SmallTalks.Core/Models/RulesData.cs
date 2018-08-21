using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SmallTalks.Core.Models
{
    [DataContract]
    public class RulesData
    {
        [DataMember(Name = "intents")]
        public List<Rule> Rules { get; set; }
    }

    [DataContract]
    public class Rule
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "rules")]
        public List<string> Patterns { get; set; }
        [DataMember(Name = "priority")]
        public int Priority { get; set; }
    }
}
