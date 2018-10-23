using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SmallTalks.Core.Models
{
    [DataContract]
    public class Analysis
    {
        [DataMember(Name = "input")]
        public string Input { get; internal set; }
        [DataMember(Name = "matchesCount")]
        public int MatchesCount { get => Matches == null ? 0 : Matches.Count; }
        [DataMember(Name = "matches")]
        public List<MatchData> Matches { get; internal set; }

        /// <summary>
        /// Input with placeholders
        /// </summary>
        [DataMember(Name = "markedInput")]
        public string MarkedInput { get; set; }

        /// <summary>
        /// Input without placeholders
        /// </summary>
        [DataMember(Name = "cleanedInput")]
        public string CleanedInput { get; set; }


        /// <summary>
        /// Indicates cleaned ratio above 0.5
        /// </summary>
        [DataMember(Name = "useCleaned")]
        public bool? UseCleanedInput{ get; internal set; }

        /// <summary>
        /// CleanedInput divided by Input 
        /// </summary>
        [DataMember(Name = "cleanedRatio")]
        public float? CleanedInputRatio { get; internal set; }

        /// <summary>
        /// Input without placeholders and stopwords (beta-feature)
        /// </summary>
        [DataMember(Name = "relevantInput")]
        public string RelevantInput { get; internal set; }

        [DataMember(Name = "cursed")]
        public bool HaveCursedWords { get; internal set; }
    }
}
