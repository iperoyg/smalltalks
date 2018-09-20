using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser
{
    public class TerminalSequence
    {
        public IList<Terminal> Sequence { get; set; }
        public string ReadableSequence => string.Join(" ", Sequence.Select(s => s.Pattern));

        public bool Evalute(string input, Grammar grammar)
        {
            var tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var sequence = Sequence.ToArray();
            int matches = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                var term = sequence[i];
                for (int j = 0; j < tokens.Length; j++)
                {
                    var token = tokens[j];
                    var match = CheckMatch(term, token, string.Join(" ", tokens, j, tokens.Length), grammar);
                }
            }
            return false;
        }

        private bool CheckMatch(Terminal term, string token, string input, Grammar grammar)
        {
            switch (term.Type)
            {
                case TerminalType.Simple:
                    return term.Pattern.Equals(token, StringComparison.InvariantCultureIgnoreCase);
                case TerminalType.Regex:
                    return Regex.IsMatch(token, term.Pattern);
                case TerminalType.Variable:
                    var rule = grammar.Rules.FirstOrDefault(r => r.Variable.Pattern == term.Pattern);
                    foreach (var sequence in rule.TerminalsSequence)
                    {
                        var result = sequence.Evalute(input, grammar);
                        if(result)
                        {
                            return true;
                        }
                    }
                    return false;

                default:
                    return false;
            }
        }

    }
}
