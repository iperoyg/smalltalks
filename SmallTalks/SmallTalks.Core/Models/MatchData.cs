using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class MatchData
    {
        public string SmallTalk { get; internal set; }
        public string Value { get; internal set; }
        public int? Index { get; internal set; }
        public int? Length { get; internal set; }
    }
}
