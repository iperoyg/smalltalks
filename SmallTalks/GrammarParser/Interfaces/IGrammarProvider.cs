using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Interfaces
{
    public interface IGrammarProvider
    {
        Grammar GetGrammar(GrammarSource source);
    }
}
