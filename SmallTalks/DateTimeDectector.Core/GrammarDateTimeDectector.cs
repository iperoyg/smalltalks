using DateTimeDectector.Core.Interfaces;
using DateTimeDectector.Domain;
using GrammarParser.Interfaces;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DateTimeDectector.Core
{
    public class GrammarDateTimeDectector : IDateTimeDectector
    {
        private readonly IGrammarSourceProvider _sourceProvider;
        private readonly IGrammarProvider _grammarProvider;
        private readonly IGrammarParserService _grammarService;
        private readonly IGrammarDateTimeDectectionEvaluator _evaluator;
        private Grammar _grammar = null;

        public GrammarDateTimeDectector(
            IGrammarSourceProvider sourceProvider,
            IGrammarProvider grammarProvider,
            IGrammarParserService grammarService,
            IGrammarDateTimeDectectionEvaluator evaluator)
        {
            _sourceProvider = sourceProvider;
            _grammarProvider = grammarProvider;
            _grammarService = grammarService;
            _evaluator = evaluator;
        }

        public List<DateTimeDectected> Detect(string input)
        {
            var grammar = GetGrammar();
            var matches = _grammarService.GetMatches(grammar, input);
            return _evaluator.Evaluate(matches);
        }

        public Task<List<DateTimeDectected>> DetectAsync(string input, CancellationToken cancellationToken)
        {
            return Task.FromResult(Detect(input));
        }

        private Grammar GetGrammar()
        {
            if(_grammar == null)
            {
                _grammar = _grammarProvider.GetGrammar(_sourceProvider.GetSource());
            }
            return _grammar;
        }
    }
}
