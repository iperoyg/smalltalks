using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser
{
    public class Terminal
    {

        public string Pattern { get; set; }
        public TerminalType Type { get; set; }
        public string Value { get; set; }

        public static Terminal FromText(string input, TerminalType type = TerminalType.Simple)
        {
            var pattern = input;

            var match = Regex.Match(input, Grammar.RegexTerminalPattern);

            if (match.Success)
            {
                pattern = match.Groups["regex"].Value;
                type = TerminalType.Regex;
            }
            return new Terminal
            {
                Pattern = pattern,
                Type = type
            };
        }

        public static Terminal ExtractVariableFromStart(string input)
        {
            var match = Regex.Match(input, $"^(?<{nameof(Terminal)}>{Grammar.VariablePattern}){Grammar.RuleStartVariableTokenSeparador}");
            if (match.Success)
            {
                var value = match.Groups[nameof(Terminal)].Value;
                var variable = FromText(value, TerminalType.Variable);
                return variable;
            }
            throw new GrammarInvalidInputFormatException();
        }

        public static Terminal ExtractVariableFromMiddle(string input)
        {
            var text = input.Trim();
            var match = Regex.Match(text, $"^(?<{nameof(Terminal)}>{Grammar.VariablePattern})$");
            if (match.Success)
            {
                var value = match.Groups[nameof(Terminal)].Value;
                return FromText(value, TerminalType.Variable);
            }
            return null;
        }
    }



    public enum TerminalType
    {
        Simple,
        Regex,
        Variable
    }
}
