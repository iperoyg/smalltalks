using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser.Models
{
    public class Rule
    {
        public Terminal Variable { get; set; }
        public IEnumerable<TerminalSequence> TerminalsSequence { get; set; }
        public RuleType Type { get; set; }
        public List<string> ExpandedRule { get; internal set; }
        public string ReadableRule => $"{Variable.Pattern} {string.Join(" | ", TerminalsSequence.Select(t => t.ReadableSequence))}";
        
        public Match Match(string input)
        {
            foreach (var rule in ExpandedRule)
            {
                var match = Regex.Match(input, rule);
                if (match.Success)
                    return match;
            }
            return null;
        }

    }

    public enum RuleType
    {
        Normal,
        Start
    }
}
