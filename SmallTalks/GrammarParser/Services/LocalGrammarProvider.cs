using GrammarParser.Interfaces;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrammarParser.Services
{
    public class LocalGrammarProvider : IGrammarParseProvider
    {
        private readonly IRuleParserService _ruleParser;

        public LocalGrammarProvider(IRuleParserService ruleParser)
        {
            _ruleParser = ruleParser;
        }

        public Grammar GetGrammar(GrammarSource source)
        {
            int counter = 0;
            string line;
            var grammar = new Grammar
            {
                Rules = new List<Rule>(),
                StartRules = new List<Rule>(),
                KnowTerminals = new HashSet<Terminal>(),
                KnowVariables = new HashSet<Terminal>()
            };

            using (var file = new StreamReader(source.Source))
            {
                while ((line = file.ReadLine()) != null)
                {
                    counter++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var rule = _ruleParser.Parse(line, grammar.KnowTerminals, grammar.KnowVariables);
                    if(rule.Type == RuleType.Start)
                    {
                        grammar.StartRules.Add(rule);
                    }
                    grammar.Rules.Add(rule);
                }
            }

            return grammar;

        }
    }
}
