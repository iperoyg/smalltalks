using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Interfaces
{
    public interface IGrammarParseProvider
    {
        Grammar GetGrammar(GrammarSource source);
    }
}
