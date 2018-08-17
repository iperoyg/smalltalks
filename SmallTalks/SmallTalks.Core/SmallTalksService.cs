using SmallTalks.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core
{
    public class SmallTalksService
    {
        public const string SM_GOOD_MORNING_REGEX = "bom\\s*dia";
        public const string SM_GOOD_NIGHT_REGEX = "boa\\s*noite";

        private readonly SmallTalksIntent GoodMorningRegex;
        private readonly SmallTalksIntent GoodNightRegex;

        private readonly List<SmallTalksIntent> Intents;

        public SmallTalksService()
        {
            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

            GoodMorningRegex = new SmallTalksIntent { Name = nameof(GoodMorningRegex), Regex = new Regex(SM_GOOD_MORNING_REGEX, options) };
            GoodNightRegex = new SmallTalksIntent { Name = nameof(GoodNightRegex), Regex = new Regex(SM_GOOD_NIGHT_REGEX, options) };

            Intents = new List<SmallTalksIntent>
            {
                GoodMorningRegex, GoodNightRegex
            };
        }

        public Analysis Detect(string input)
        {
            var analysis = new Analysis
            {
                Input = input,
                Matches = new List<MatchData>()
            };

            foreach (var intent in Intents)
            {
                var matches = intent.Regex.Matches(input);

                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        analysis.Matches.Add(new MatchData
                        {
                            SmallTalk = intent.Name,
                            Value = m.Value,
                            Index = m.Index,
                            Lenght = m.Length,
                        });
                    }
                }
            }

            return analysis;
        }

    }

    public class SmallTalksIntent
    {
        public string Name { get; set; }
        public Regex Regex { get; set; }
    }
}
