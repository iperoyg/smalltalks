using NUnit.Framework;
using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Tests.Services
{
    [TestFixture]
    class PreProcessorBuilderTest
    {
        [TestCase("aaaaaaaaaa", "aa")]
        [TestCase("oiiiiiiiii!!!! bom diiia", "oii! bom diia")]
        [TestCase("olááá!!!!!         tudo beeeeeeeem?", "oláá! tudo beem?")]
        [TestCase("ooooia", "ooia")]
        public void When_PreProcess_RepeteadChar_ShouldRemoveThen(string input, string expectedOutput)
        {
            var preprocessor = new PreProcess
            {
                Input = input
            }
            .RemoveRepeteadChars();

            Assert.AreEqual(expectedOutput, preprocessor.Output);
        }
    }
}
