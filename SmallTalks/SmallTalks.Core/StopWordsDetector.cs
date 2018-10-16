using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        private async Task Load()
        {
            if (_stopWordsRegex == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var swStream = assembly.GetManifestResourceStream($"SmallTalks.Core.{_sourceProvider.GetSourceProvider().StopWords}");
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
                    _stopWordsRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                }
            }
        }

        public async Task<bool> HaveStopWordsAsync(string input)
        {
            await Load();
            var isMatch = _stopWordsRegex.IsMatch(input);
            return isMatch;
        }

        public async Task<string> RemoveStopWordsAsync(string input)
        {
            await Load();
            var removed = _stopWordsRegex.Replace(input, string.Empty);
            return removed.Trim();
        }
    }
}
