using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammarParser
{
    public class RuleParser
    {
        public static Rule Parse(string rawRuleText, ISet<Terminal> knowTerminals, ISet<Terminal> knowVariables)
        {
            var rule = new Rule { Type = RuleType.Normal };
            var text = rawRuleText.Trim();
            if (text.StartsWith(Grammar.StartToken))
            {
                rule.Type = RuleType.Start;
                text = text.Substring(Grammar.StartToken.Length);
            }
            var variable = Terminal.ExtractVariableFromStart(text);
            knowVariables.Add(variable);
            rule.Variable = variable;

            text = text.Substring(variable.Pattern.Length + Grammar.RuleStartVariableTokenSeparador.Length);
            var tokensRules = text.Split(new string[] { Grammar.SplitToken }, StringSplitOptions.RemoveEmptyEntries);

            var terminalsSequences = new List<TerminalSequence>();
            foreach (var tokensRule in tokensRules)
            {
                var tokens = tokensRule.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var terminalsSequence = new TerminalSequence { Sequence = new List<Terminal>() };
                foreach (var token in tokens)
                {
                    var middleVariable = Terminal.ExtractVariableFromMiddle(token);
                    if (middleVariable != null) // 'item' isn't a terminal
                    {
                        if (!knowVariables.Any(v => v.Pattern == middleVariable.Pattern))
                        {
                            throw new GrammarInvalidInputFormatException();
                        }
                        terminalsSequence.Sequence.Add(middleVariable);
                    }
                    else
                    {
                        var terminal = Terminal.FromText(token);
                        knowTerminals.Add(terminal);
                        terminalsSequence.Sequence.Add(terminal);
                    }

                }
                terminalsSequences.Add(terminalsSequence);
            }
            rule.TerminalsSequence = terminalsSequences;

            return rule;
        }

    }
}
