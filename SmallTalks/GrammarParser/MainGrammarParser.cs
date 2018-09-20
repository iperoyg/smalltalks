using Entities.Domain.Extensions;
using Entities.Domain.Models;
using GrammarParser.Interfaces;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser
{
    public class MainGrammarParser : IGrammarParserService
    {
        private readonly IRuleParserService _ruleParser;

        public MainGrammarParser(IRuleParserService ruleParser)
        {
            _ruleParser = ruleParser;
        }

        public List<EvaluatedRule> GetMatches(Grammar grammar, string input)
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

            ExpandAllRules(grammar);
            bool atLeastOneMatch = true;

            while (atLeastOneMatch)
            {
                atLeastOneMatch = false;
                foreach (var rule in grammar.StartRules)
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

        public List<string> GetExpandedRulesList(Grammar grammar)
        {
            ExpandAllRules(grammar);
            var list = new List<string>();
            foreach (var rule in grammar.StartRules)
            {
                list.AddRange(rule.ExpandedRule.ToList());
            }
            return list;
        }
        private List<string> ExpandAllRules(Grammar grammar)
        {
            var allRules = new List<string>();
            foreach (var rule in grammar.StartRules)
            {
                allRules.AddRange(_ruleParser.ExpandRule(grammar, rule));
            }
            return allRules;
        }



    }
}
