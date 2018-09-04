using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core
{
    public class StopWordsDetector : IStopWordsDetector
    {
        private readonly ISourceProviderService _sourceProvider;
        private Regex _stopWordsRegex = null;

        public StopWordsDetector(ISourceProviderService sourceProvider)
        {
            _sourceProvider = sourceProvider;
        }

        private void Load()
        {
            if (_stopWordsRegex == null)
            {
                var dataAsString = File.ReadAllText(_sourceProvider.GetSourceProvider().StopWords, Encoding.UTF8);
                StringBuilder pattern = new StringBuilder();
                var lines = dataAsString.Split('\n');
                foreach (var line in lines)
                {
                    var cleanLine = line.Trim();
                    pattern.Append($"\\b{cleanLine}\\b|");
                }
                pattern.Remove(pattern.Length - 1, 1);
                _stopWordsRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            }
        }

        public bool HaveStopWords(string input)
        {
            Load();
            var isMatch = _stopWordsRegex.IsMatch(input);
            return isMatch;
        }

        public string RemoveStopWords(string input)
        {
            Load();
            var removed = _stopWordsRegex.Replace(input, string.Empty);
            return removed.Trim();
        }
    }
}
