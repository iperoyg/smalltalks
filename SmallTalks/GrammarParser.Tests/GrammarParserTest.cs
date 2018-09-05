using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Tests
{
    [TestFixture]
    public class GrammarParserTest
    {
        [Test]
        public void GetGrammarFromLocal()
        {
            var provider = new LocalGrammarProvider();
            var source = new GrammarSource { Source = "date_gramar.txt" };
            var grammar = provider.GetGrammar(source);
        }

    }
}
