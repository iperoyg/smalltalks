using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class Analysis
    {
        public string Input { get; internal set; }
        public int MatchesCount { get => Matches == null ? 0 : Matches.Count; }
        public List<MatchData> Matches { get; internal set; }
        public string AnalysedInput { get; set; }
    }
}
