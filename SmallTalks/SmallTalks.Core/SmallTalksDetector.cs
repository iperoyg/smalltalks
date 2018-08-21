using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core
{
    public class SmallTalksDetector : ISmallTalksDetector
    {
        private readonly IDetectorDataProviderService _detectorDataProvider;
        private readonly ISourceProviderService _sourceProvider;

        public SmallTalksDectectorData DectectorData { get; private set; }

        public SmallTalksDetector(IDetectorDataProviderService detectorDataProvider, ISourceProviderService sourceProvider)
        {
            _detectorDataProvider = detectorDataProvider;
            _sourceProvider = sourceProvider;
        }

        public void Init()
        {
            DectectorData = DectectorData ?? _detectorDataProvider.GetSmallTalksDetectorDataFromSource(_sourceProvider.GetSourceProvider());
        }

        public Analysis Detect(string input)
        {
            Init();
            var analysis = new Analysis
            {
                Input = input,
                Matches = new List<MatchData>()
            };

            DectectorData.SmallTalksIntents = DectectorData.SmallTalksIntents.OrderBy(i => i.Priority).ToList();

            var parsedInput = input;
            foreach (var intent in DectectorData.SmallTalksIntents)
            {
                var matches = intent.Regex.Matches(parsedInput);

                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        parsedInput = parsedInput.Replace(index: m.Index, length: m.Length, replacement: '_');
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

            analysis.AnalysedInput = parsedInput;

            return analysis;
        }

    }


}
