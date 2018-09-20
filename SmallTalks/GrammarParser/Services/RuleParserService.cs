using GrammarParser.Exceptions;
using GrammarParser.Interfaces;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser.Services
{
    public class RuleParserService : IRuleParserService
    {
        private readonly ITerminalParserService _terminalParser;

        public RuleParserService(ITerminalParserService terminalParser)
        {
            _terminalParser = terminalParser;
        }

        public Rule Parse(string rawRuleText, ISet<Terminal> knowTerminals, ISet<Terminal> knowVariables)
        {
            var rule = new Rule { Type = RuleType.Normal };
            var text = rawRuleText.Trim();
            if (text.StartsWith(Grammar.StartToken))
            {
                rule.Type = RuleType.Start;
                text = text.Substring(Grammar.StartToken.Length);
            }
            var variable = _terminalParser.ExtractVariableFromStart(text);
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
                    var middleVariable = _terminalParser.ExtractVariableFromMiddle(token);
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
                        var terminal = _terminalParser.FromText(token);
                        knowTerminals.Add(terminal);
                        terminalsSequence.Sequence.Add(terminal);
                    }

                }
                terminalsSequences.Add(terminalsSequence);
            }
            rule.TerminalsSequence = terminalsSequences;

            return rule;
        }

        public List<string> ExpandRule(Grammar grammar, Rule rule)
        {
            if (rule.ExpandedRule != null)
            {
                return rule.ExpandedRule;
            }

            var allSequences = new List<string>();
            foreach (var sequence in rule.TerminalsSequence)
            {
                allSequences.AddRange(ExpandSequence(grammar, sequence));
            }

            rule.ExpandedRule = allSequences;
            return allSequences;
        }

        private List<string> ExpandSequence(Grammar grammar, TerminalSequence terminalSequence)
        {
            var allTerminals = new List<string>();
            foreach (var terminal in terminalSequence.Sequence)
            {
                var sequence = new List<string>();
                if (terminal.Type == TerminalType.Variable)
                {
                    var rule = GetRule(grammar, terminal);
                    sequence = rule.ExpandedRule?.ToList() ?? ExpandRule(grammar, rule).ToList();
                }
                else
                {
                    sequence.Add(terminal.Pattern);
                }

                var prefixes = new List<string>();
                foreach (var terminals in allTerminals)
                {
                    prefixes.Add(terminals);
                }

                allTerminals.Clear();
                foreach (var item in sequence)
                {
                    if (prefixes.Any())
                    {
                        foreach (var prefix in prefixes)
                        {
                            allTerminals.Add($"\\b{prefix}\\b\\s?\\b{item}\\b");
                        }
                    }
                    else
                    {
                        allTerminals.Add($"\\b{item}\\b");
                    }
                }
            }
            allTerminals = allTerminals.Select(s => Regex.Replace(s, "(\\\\b)+", "\\b")).ToList();
            return allTerminals;
        }

        private Rule GetRule(Grammar grammar, Terminal variableTerminal)
        {
            if (variableTerminal.Type != TerminalType.Variable)
            {
                throw new ArgumentException("Terminal must be a variable", nameof(variableTerminal));
            }
            return grammar.Rules.FirstOrDefault(r => r.Variable.Pattern == variableTerminal.Pattern);
        }


    }
}
