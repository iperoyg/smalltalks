using SmallTalks.Core.Models;
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

            return stIntent;
        }

        public SmallTalksDectectorData ToDetectorData(RulesData rules)
        {
            return new SmallTalksDectectorData
            {
                SmallTalksIntents = rules.Rules.Select(r => ToIntent(r)).ToList()
            };            
        }
    }
}
