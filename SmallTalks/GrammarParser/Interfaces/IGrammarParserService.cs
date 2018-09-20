using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Interfaces
{
    public interface IGrammarParserService
    {
        List<EvaluatedRule> GetMatches(Grammar grammar, string input);
        List<string> GetExpandedRulesList(Grammar grammar);
    }
}
