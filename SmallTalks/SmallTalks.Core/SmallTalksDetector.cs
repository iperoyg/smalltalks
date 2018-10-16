using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Entities.Domain.Extensions;
using Entities.Domain.Models;
using System.Threading.Tasks;
using SmallTalks.Core.Services.Interfaces;

namespace SmallTalks.Core
{
    public class SmallTalksDetector : ISmallTalksDetector
    {
        private readonly IDetectorDataProviderService _detectorDataProvider;
        private readonly ISourceProviderService _sourceProvider;
        private readonly IWordDetectorFactory _wordDetectorFactory;

        private IWordsDetector _stopWordsDetector;
        private IWordsDetector _curseWordsDetector;

        public SmallTalksDectectorData DectectorData { get; private set; }

        public SmallTalksDetector(
            IDetectorDataProviderService detectorDataProvider, 
            ISourceProviderService sourceProvider,
            IWordDetectorFactory wordDetectorFactory)
        {
            _detectorDataProvider = detectorDataProvider;
            _sourceProvider = sourceProvider;
            _wordDetectorFactory = wordDetectorFactory;
        }

        public async Task Init()
        {
            _stopWordsDetector = await _wordDetectorFactory.GetWordsDetectorAsync(WordDetectorType.Stopwords);
            _curseWordsDetector = await _wordDetectorFactory.GetWordsDetectorAsync(WordDetectorType.Cursewords);
            DectectorData = DectectorData ?? await _detectorDataProvider.GetSmallTalksDetectorDataFromSourceAsync(_sourceProvider.GetSourceProvider());
        }

        public async Task<Analysis> DetectAsync(string input)
        {
            var preProcess = new InputProcess
            {
                Input = input
            }
            .RemoveRepeteadChars()
            .ToLower();

            await Init();
            var analysis = new Analysis
            {
                Input = preProcess.Output,
                Matches = new List<MatchData>()
            };

            DectectorData.SmallTalksIntents = DectectorData.SmallTalksIntents.OrderBy(i => i.Priority).ToList();

            var parsedInput = input;
            var haveCursedWords = false;
            (parsedInput, haveCursedWords) = await _curseWordsDetector.ReplaceWordsAsync(parsedInput, InputProcess.Placeholder);
            analysis.HaveCursedWords = haveCursedWords;

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
                .RemovePunctuation()
                .RemoveRepeteadChars()
                .RemovePlaceholder()
                .Output;
            
            analysis.RelevantInput = InputProcess.FromString(await _stopWordsDetector.RemoveWordsAsync(analysis.CleanedInput))
                .RemoveRepeteadChars()
                .Output;

            return analysis;
        }

    }


}

