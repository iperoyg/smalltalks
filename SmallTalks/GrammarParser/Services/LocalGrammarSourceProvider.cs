using GrammarParser.Interfaces;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Services
{
    public class LocalGrammarSourceProvider : IGrammarSourceProvider
    {
        public GrammarSource GetSource()
        {
            return new GrammarSource { Source = "date_gramar.txt" };
        }
    }
}
