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
using Serilog;
using SmallTalks.Core.Services.Interfaces;

namespace SmallTalks.Core.Services
{
    public class WordsDetectorBase : IWordsDetector
    {
        private readonly ISourceProviderService _sourceProvider;
        private readonly ILogger _logger;
        private Regex _wordsRegex = null;

        public string WordFile { get; private set; }

        public WordsDetectorBase(
            ISourceProviderService sourceProvider,
            ILogger logger)
        {
            _sourceProvider = sourceProvider;
            _logger = logger;
        }

        public async Task LoadAsync(string wordFile)
        {
            WordFile = wordFile;
            if (_wordsRegex == null)
            {
                try
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
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Unexpected fail when loading word detector file from embedded resource {wordFileType} ", wordFile);
                    throw;
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
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when replacing {wordFileType} at {sentence}", WordFile, input);
                return (input, false);
            }

        }
    }
}
