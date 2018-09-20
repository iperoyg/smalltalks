using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Domain.Extensions;
using Entities.Domain.Models;
using System.Text.RegularExpressions;

namespace GrammarParser
{
    public class Grammar
    {
        public const string StartToken = ">";
        public const string SplitToken = " | ";
        public const string RuleStartVariableTokenSeparador = ":";
        public const string VariablePattern = "\\b[A-Z][A-Z\\d_]*\\b";
        public const string RegexTerminalPattern = "^r'(?<regex>.+)'$";

        public IList<Rule> Rules { get; set; }
        public IList<Rule> StartRules { get; set; }
        public ISet<Terminal> KnowTerminals { get; set; }
        public ISet<Terminal> KnowVariables { get; set; }


        public List<EvaluatedRule> GetMatches(string input)
        {
            var matches = new List<EvaluatedRule>();

            var preProcess = new InputProcess
            {
                Input = input
            }
            .RemoveAccentuation()
            .ToLower()
            .RemoveRepeteadChars()
            .TranslateTextNumberToNumeric();

            var postProcessedInput = preProcess.Output;
            
            ExpandAllRules();
            bool atLeastOneMatch = true;

            while (atLeastOneMatch)
            {
                atLeastOneMatch = false;
                foreach (var rule in StartRules)
                {
                    var match = rule.Match(postProcessedInput);
                    if (match != null)
                    {
                        atLeastOneMatch = true;

                        postProcessedInput = postProcessedInput.Replace(index: match.Index, length: match.Length, replacement: '_');

                        matches.Add(new EvaluatedRule
                        {
                            Rule = rule,
                            Input = input,
                            Value = match.Value
                        });
                    }
                }
            }

            return matches;
        }



        public List<string> ExpandAllRules()
        {
            var allRules = new List<string>();
            foreach (var rule in StartRules)
            {
                allRules.AddRange(ExpandRule(rule));
            }
            return allRules;
        }

        public List<string> GetExpandedRulesList()
        {
            var list = new List<string>();
            foreach (var rule in StartRules)
            {
                list.AddRange(rule.ExpandedRule.ToList());
            }
            return list;
        }

        public IList<string> ExpandRule(Rule rule)
        {
            if (rule.ExpandedRule != null)
            {
                return rule.ExpandedRule;
            }

            var allSequences = new List<string>();
            foreach (var sequence in rule.TerminalsSequence)
            {
                allSequences.AddRange(ExpandSequence(sequence));
            }

            allSequences = allSequences.Select(s => Regex.Replace(s, "(\\\\b)+", "\\b")).ToList();

            rule.ExpandedRule = allSequences;
            return allSequences;
        }

        public IList<string> ExpandSequence(TerminalSequence terminalSequence)
        {
            var allTerminals = new List<string>();
            foreach (var terminal in terminalSequence.Sequence)
            {
                var sequence = new List<string>();
                if (terminal.Type == TerminalType.Variable)
                {
                    var rule = GetRule(terminal);
                    sequence = rule.ExpandedRule?.ToList() ?? ExpandRule(rule).ToList();
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
            return allTerminals;

        }

        public Rule GetRule(Terminal variableTerminal)
        {
            if (variableTerminal.Type != TerminalType.Variable)
            {
                throw new ArgumentException("Terminal must be a variable", nameof(variableTerminal));
            }
            return Rules.FirstOrDefault(r => r.Variable.Pattern == variableTerminal.Pattern);
        }

    }


}
