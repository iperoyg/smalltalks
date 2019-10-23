using Entities.Domain.Extensions;
using Entities.Domain.Models;
using NUnit.Framework;

namespace SmallTalks.Core.Tests.Services
{
    [TestFixture]
    class PreProcessorBuilderTest
    {
        [TestCase("aaaaaaaaaa", "aa")]
        [TestCase("oiiiiiiiii!!!! bom diiia", "oii! bom diia")]
        [TestCase("olááá!!!!!         tudo beeeeeeeem?", "oláá! tudo beem?")]
        [TestCase("ooooia", "ooia")]
        [TestCase("tttuddo bbbaum ayyy? ççe tta okkkk? ffffllllaaaaa cmmgggg jjjow", "tudo bbaum ay? çe ta okk? flaa cmmg jow")]
        [TestCase("eh, amanha de manha", "eh, amanha de manha")]
        [TestCase("amaama ela", "amaama ela")]
        [TestCase("aaaaamaaaaaama elaaaaa", "aamaama elaa")]
        public void When_PreProcess_RepeteadChar_ShouldRemoveThen(string input, string expectedOutput)
        {
            var preprocessor = new InputProcess
            {
                Input = input
            }
            .RemoveRepeteadChars();

            Assert.AreEqual(expectedOutput, preprocessor.Output);
        }
    }
}
