using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            grammar.ExpandAllRules();
        }

        [TestCase("quero ver esse produto amanhã", "DATA_EXATA_EXTENSO")]
        [TestCase("ontem eu fiz uma compra", "DATA_EXATA_EXTENSO")]
        [TestCase("no dia 13/05/1992", "DATA_EXATA_NUMERICA")]
        [TestCase("Em até 3 dias", "DATA_FUTURA")]
        [TestCase("Já fazem 5 dias", "DATA_PASSADA")]
        [TestCase("Eu comi a mais ou menos 3 dias atrás", "DATA_PASSADA")]
        [TestCase("Foi uns 3 dias atrás", "DATA_PASSADA")]
        [TestCase("Pois é, um dia atrás", "DATA_PASSADA")]
        [TestCase("Já tem 10 dias", "DATA_PASSADA")]
        [TestCase("Já fazem 4 horas", "TEMPO_PASSADO")]
        [TestCase("Daqui 1 mes", "DATA_FUTURA")]
        [TestCase("Após 10 minutos", "TEMPO_FUTURO")]
        [TestCase("As 2 horas", "TEMPO_EXATO_EXTENSO")]
        [TestCase("13 de Jun de 1992", "DATA_EXATA_NUMERICA")]
        [TestCase("13 de Junho de 1992", "DATA_EXATA_NUMERICA")]
        [TestCase("13 de 6 de 1992", "DATA_EXATA_NUMERICA")]
        [TestCase("desse ano", "DATA_EXATA_EXTENSO")]
        [TestCase("desse dia", "DATA_EXATA_EXTENSO")]
        [TestCase("dia de hoje", "DATA_EXATA_EXTENSO")]
        [TestCase("hoje", "DATA_EXATA_EXTENSO")]
        [TestCase("agora", "TEMPO_EXATO_EXTENSO")]
        public void When_ScanGrammar_ShouldMatch(string input, string expectedVariableRule)
        {
            var provider = new LocalGrammarProvider();
            var source = new GrammarSource { Source = "date_gramar.txt" };
            var grammar = provider.GetGrammar(source);
            grammar.ExpandAllRules();

            var matches = grammar.GetMatches(input);

            Assert.AreEqual(1, matches.Count);
            var match = matches.First();
            Assert.AreEqual(expectedVariableRule, match.Rule.Variable.Pattern);


        }

    }
}
