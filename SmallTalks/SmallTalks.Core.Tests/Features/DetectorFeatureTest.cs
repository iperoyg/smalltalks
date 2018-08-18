using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SmallTalks.Core.Tests.Features
{
    [TestFixture]
    public class DetectorFeatureTest
    {
        public static IEnumerable<TestCaseData> DataSources
        {
            get
            {
                yield return new TestCaseData("bom dia", 1);
                yield return new TestCaseData("boa tarde", 1);
                yield return new TestCaseData("boa noite", 1);
            }
        }

        public static IEnumerable<TestCaseData> GetEnsureMatchData()
        {
            List<TestCaseData> testCaseList = new List<TestCaseData>();
            using (var reader = new StreamReader("EnsureMatchSource.tsv"))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var matchData = new EnsureSmallTalkMatchData { ExpectedMatches = new List<ExpectedSmallTalkMatch>() };
                    var parts = line.Split("\t");
                    var input = parts[0];

                    matchData.Input = input;

                    var expectedParts = parts[1].Split(',');
                    foreach (var part in expectedParts)
                    {
                        var match = part.Split(':');
                        var name = match[0].Trim();
                        var count = Convert.ToInt32(match[1].Trim());
                        matchData.ExpectedMatches.Add(new ExpectedSmallTalkMatch
                        {
                            Name = name,
                            Count = count
                        });

                    }
                    testCaseList.Add(new TestCaseData(matchData));
                }
            }

            foreach (var testCase in testCaseList)
            {
                yield return testCase;
            }
        }

        [Test, TestCaseSource("GetEnsureMatchData")]
        public void EnsureMatch(EnsureSmallTalkMatchData matchData)
        {
            var serviceCollection = new ServiceCollection();
            ContainerProvider.RegisterTypes(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var st = serviceProvider.GetService<ISmallTalksDetector>();
            var analysis = st.Detect(matchData.Input);

            var count = matchData.ExpectedMatches.Sum(m => m.Count);
            
            foreach (ExpectedSmallTalkMatch expectedMatch in matchData.ExpectedMatches)
            {
                var stMatchesCount = analysis.Matches.Where(m => m.SmallTalk.Equals(expectedMatch.Name, StringComparison.OrdinalIgnoreCase)).Count();
                Assert.That(expectedMatch.Count == stMatchesCount, () => $"The phrase '{matchData.Input}' doesn't contains {expectedMatch.Count} {expectedMatch.Name} smalltalks. Found {stMatchesCount} instead.");
            }
            Assert.That(count == analysis.MatchesCount, () => $"The phrase '{matchData.Input}' doesn't contains {count} small talks. Found {analysis.MatchesCount} instead. ");
        }
    }

    public class EnsureSmallTalkMatchData
    {
        public string Input { get; set; }
        public List<ExpectedSmallTalkMatch> ExpectedMatches { get; set; }
    }

    public class ExpectedSmallTalkMatch
    {
        public string Name { get; set; }
        public int Count { get; set; }

    }
}
