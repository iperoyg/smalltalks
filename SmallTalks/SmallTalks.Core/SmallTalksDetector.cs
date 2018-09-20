using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Entities.Core.Extensions;
using Entities.Core.Models;
using Entities.Core.Services;

namespace SmallTalks.Core
{
    public class SmallTalksDetector : ISmallTalksDetector
    {
        private readonly IDetectorDataProviderService _detectorDataProvider;
        private readonly ISourceProviderService _sourceProvider;
        private readonly IStopWordsDetector _stopWordsDetector;

        public SmallTalksDectectorData DectectorData { get; private set; }

        public SmallTalksDetector(
            IDetectorDataProviderService detectorDataProvider, 
            ISourceProviderService sourceProvider,
            IStopWordsDetector stopWordsDetector)
        {
            _detectorDataProvider = detectorDataProvider;
            _sourceProvider = sourceProvider;
            _stopWordsDetector = stopWordsDetector;
        }

        public void Init()
        {
            DectectorData = DectectorData ?? _detectorDataProvider.GetSmallTalksDetectorDataFromSource(_sourceProvider.GetSourceProvider());
        }

        public Analysis Detect(string input)
        {
            var preProcess = new InputProcess
            {
                Input = input
            }
            .RemoveRepeteadChars()
            .ToLower();

            Init();
            var analysis = new Analysis
            {
                Input = preProcess.Output,
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
                        parsedInput = parsedInput.Replace(index: m.Index, length: m.Length, replacement: InputProcess.Placeholder);
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
            analysis.CleanedInput = InputProcess.FromString(parsedInput)
                .RemovePlaceholder()
                .RemovePunctuation()
                .Output;
            analysis.RelevantInput = _stopWordsDetector.RemoveStopWords(analysis.CleanedInput);

            return analysis;
        }

    }


}
