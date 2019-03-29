using SmallTalks.Core.Models;
using SmallTalks.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core.Services
{
    public class ModelConversionService : IConversionService
    {
        public SmallTalksIntent ToIntent(Rule rule)
        {
            var stIntent = new SmallTalksIntent
            {
                Name = rule.Name
            };

            var patters = rule.Patterns
                .Select(p => Regex.IsMatch(p, "^\\w") ? $"\\b{p}" : p)
                .Select(p => Regex.IsMatch(p, "\\w$") ? $"{p}\\b" : p)
                .ToList();
            var regexPattern = string.Join('|', patters);

            stIntent.Regex = new Regex(regexPattern, Configuration.ST_REGEX_OPTIONS);
            stIntent.Priority = rule.Priority;

            return stIntent;
        }

        public SmallTalksIntent ToIntentv2(Rule rule)
        {
            var stIntent = new SmallTalksIntent
            {
                Name = rule.Name
            };

            var patterns = rule.Patterns
                .Select(p => Regex.IsMatch(p, "^\\w") ? $"\\b{p}" : p)
                .Select(p => Regex.IsMatch(p, "\\w$") ? $"{p}\\b" : p)
                .Select(p => RegexAdd(rule.Position, p))
                .ToList();
            var finalPatterns = patterns.SelectMany(x => x).ToList();
            var regexPattern = string.Join('|', finalPatterns);

            stIntent.Regex = new Regex(regexPattern, Configuration.ST_REGEX_OPTIONS);
            stIntent.Priority = rule.Priority;

            return stIntent;
        }

        public SmallTalksDectectorData ToDetectorData(RulesData rules)
        {
            return new SmallTalksDectectorData
            {
                SmallTalksIntents = rules.Rules.Select(r => ToIntent(r)).ToList()
            };            
        }

        public SmallTalksDectectorData ToDetectorDatav2(RulesData rules)
        {
            return new SmallTalksDectectorData
            {
                SmallTalksIntents = rules.Rules.Select(r => ToIntentv2(r)).ToList()
            };
        }

        #region private methods
        private List<string> RegexAdd(List<string> positions, string pattern)
        {
            var finalPatterns = new List<string>();
            foreach (var position in positions)
            {
                switch (position)
                {
                    case "alone":
                        finalPatterns.Add(string.Concat("^", pattern, "$"));
                        break;

                    case "beginning":
                        finalPatterns.Add(string.Concat("^", pattern));
                        break;

                    case "ending":
                        finalPatterns.Add(string.Concat(pattern, "$"));
                        break;

                    default:
                        break;
                }
            }

            return finalPatterns;
        }
        #endregion
    }
}
