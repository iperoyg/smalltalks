using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Domain.Extensions;

namespace Entities.Domain.Tests
{
    [TestFixture]
    public class ExtensionsTest
    {
        [TestCase("eu quero trinta e um desse, do outro, oito, do segundo, podem ser mais cento e tres e quatro no fim",
            "eu quero 31 desse, do outro, 8, do 2, podem ser mais 103 e 4 no fim")]
        [TestCase("um, dois, tres", "1, 2, 3")]
        [TestCase("um e dois e tres", "1 e 2 e 3")]
        [TestCase("um dois tres", "1 2 3")]
        [TestCase("vinte, trinta, quarenta", "20, 30, 40")]
        [TestCase("vinte e trinta e quarenta", "20 e 30 e 40")]
        [TestCase("vinte trinta quarenta", "20 30 40")]
        [TestCase("cem e duzentos e trezentos", "100 e 200 e 300")]
        [TestCase("cem, quatro e vinte um", "100, 4 e 21")]
        [TestCase("cem, quatro e vinte dois", "100, 4 e 22")]
        [TestCase("cem, quatro e vinte e tres", "100, 4 e 23")]
        [TestCase("cento e quarenta e oito bananas", "148 bananas")]
        [TestCase("quero cento e um", "quero 101")]
        [TestCase("quero duzentos e mais trinta", "quero 200 e mais 30")]
        [TestCase("quero trinta e mais duzentos e dois", "quero 30 e mais 202")]
        [TestCase("so trinta e 2", "so 32")]
        [TestCase("so 2 e trinta", "so 2 e 30")]
        [TestCase("faz cento 1", "faz 101")]
        [TestCase("mil", "1000")]
        [TestCase("2 mil", "2000")]
        [TestCase("dez mil", "10000")]
        [TestCase("cem mil", "100000")]
        [TestCase("cem milhoes", "100000000")]
        [TestCase("cem e quatro", "100 e 4")]
        [TestCase("treze e cem", "13 e 100")]
        [TestCase("quatro e 100", "4 e 100")]
        [TestCase("cem e treze", "100 e 13")]
        [TestCase("quero cem e treze milhares", "quero 100 e 13000")]
        [TestCase("cem milhoes", "100000000")]
        [TestCase("mil e oito", "1008")]
        [TestCase("quero mil de um e oito mil e treze do outro", "quero 1000 de 1 e 8013 do outro")]
        [TestCase("treze de junho de mil novecentos e noventa e dois eh meu nasc", "13 de junho de 1992 eh meu nasc")]
        [TestCase("gostaria de 2 datas, daqui quatro dias e daqui oito dias", "gostaria de 2 datas, daqui 4 dias e daqui 8 dias")]
        [TestCase("13 do 6 de 1992", "13 do 6 de 1992")]
        [TestCase("treze do seis de mil novecentos e noventa e dois", "13 do 6 de 1992")]
        [TestCase("dia 13 do 6 de 1992", "dia 13 do 6 de 1992")]
        [TestCase("dia 13 do 06 de 1992", "dia 13 do 6 de 1992")]
        [TestCase("dia 13/06/1992", "dia 13/6/1992")]
        [TestCase("dia 04 do 12 de 1992", "dia 4 do 12 de 1992")]
        public void When_Convert_RawFull_To_Int_Should_Works(string input, string expected)
        {
            var processed = input.ParseIntegersFromRaw();

            Assert.AreEqual(expected, processed);
        }

    }
}
