using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Interfaces
{
    public interface IRuleParserService
    {
        Rule Parse(string rawRuleText, ISet<Terminal> knowTerminals, ISet<Terminal> knowVariables);
        List<string> ExpandRule(Grammar grammar, Rule rule);
    }
}
