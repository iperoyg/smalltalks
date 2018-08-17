using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Tests.Features
{
    [TestFixture]
    public class DetectorFeatureTest
    {


        [TestCase("bom dia", 1)]
        [TestCase("bom dia dia", 1)]
        [TestCase("bom   dia", 1)]
        [TestCase("bom   dia bom dia", 2)]
        [TestCase("bom dia bom dia", 2)]
        [TestCase("bomdia bom dia", 2)]
        [TestCase("bomdia", 1)]
        [TestCase("bomdia,boatarde, boa noite brasil", 2)]
        public void EnsureMatch(string input, int expectedCount)
        {
            var st = new SmallTalksService();
            var analysis = st.Detect(input);
            Assert.AreEqual(expectedCount, analysis.MatchesCount);
        }
    }
}
