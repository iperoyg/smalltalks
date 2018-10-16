using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Entities.Domain.Extensions;

namespace SmallTalks.Core
{
    public class WordsDetector : IWordsDetector
    {
        private readonly ISourceProviderService _sourceProvider;
        private Regex _wordsRegex = null;

        public WordsDetector(ISourceProviderService sourceProvider)
        {
            _sourceProvider = sourceProvider;
        }

        public async Task LoadAsync(string wordFile)
        {
            if (_wordsRegex == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var swStream = assembly.GetManifestResourceStream($"SmallTalks.Core.{wordFile}");
                using (var reader = new StreamReader(swStream, Encoding.UTF8))
                {
                    var dataAsString = await reader.ReadToEndAsync();
                    StringBuilder pattern = new StringBuilder();
                    var lines = dataAsString.Split('\n');
                    foreach (var line in lines)
                    {
                        var cleanLine = line.Trim();
                        pattern.Append($"\\b{cleanLine}\\b|");
                    }
                    pattern.Remove(pattern.Length - 1, 1);
                    _wordsRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                }
            }
        }

        public async Task<bool> HaveWordsAsync(string input)
        {
            var isMatch = _wordsRegex.IsMatch(input);
            return isMatch;
        }

        public async Task<string> RemoveWordsAsync(string input)
        {
            var removed = _wordsRegex.Replace(input, string.Empty);
            return removed.Trim();
        }

        public async Task<(string, bool)> ReplaceWordsAsync(string input, char placeholder)
        {
            var parsedInput = input;
            var matches = _wordsRegex.Matches(parsedInput);

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    parsedInput = parsedInput.Replace(index: m.Index, length: m.Length, replacement: placeholder);
                }
            }

            return (parsedInput, matches.Count > 0);

        }
    }
}
