using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Words;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Matching
{
    [TestFixture]
    public class WordMatchingTests
    {
        [Test]
        public async Task TestWordMatchingAgainstNPM()
        {
            var wordMatchingTestDefintions = new Dictionary<string, MatchType>
            {
                {"axios", MatchType.Exact},
                {"crikey", MatchType.Exact},
                {"hello", MatchType.Exact},
                {"axio", MatchType.Partial},
                {"faceboo", MatchType.Partial},
                {"darkly", MatchType.Partial},
                {"vixens", MatchType.None},
                {"axiosify", MatchType.None},
                {"tangleberry", MatchType.None}
            };

            var matchingTasks = wordMatchingTestDefintions
                .Select(async (pair) =>
                {
                    var matchTypeResult = await WordMatchingService.MatchWordAgainstNPM(pair.Key);

                    Assert.That(matchTypeResult, Is.EqualTo(pair.Value));
                });

            await Task.WhenAll(matchingTasks);
        }
    }
}