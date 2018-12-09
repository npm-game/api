using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.NPM;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace NPMGame.Core.Tests.Workers.Words
{
    [TestFixture]
    public class WordMatchingTests
    {
        private readonly IWordMatchingService _wordMatchingService;

        private readonly string[] _mockRemotePackages = {
            "axios",
            "axios-create",
            "crikey",
            "aussie-crikey",
            "hello",
            "hello.js",
            "facebook-sdk",
            "darkly-theme-bootstrap",
            "vixels",
            "vix",
            "tangoberry",
            "ify",
            "other",
            "dumb",
            "packages",
            "shouldnot",
            "matter"
        };

        public WordMatchingTests()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp
                .When("https://registry.npmjs.org/*")
                .Respond("application/json", JsonConvert.SerializeObject(
                    new NPMSearchResponse
                    {
                        objects = _mockRemotePackages
                            .Select(p => new NPMSearchObject
                            {
                                package = new NPMSearchObjectPackage
                                {
                                    name = p
                                }
                            })
                            .ToList()
                    })
                );

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient().Returns(mockHttp.ToHttpClient());

            _wordMatchingService = new WordMatchingService(httpClientFactory);
        }

        [Test]
        public async Task TestWordMatchingAgainstNPM()
        {
            var wordMatchingTestDefinitions = new Dictionary<string, MatchType>
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

            var matchingTasks = wordMatchingTestDefinitions
                .Select(async (pair) =>
                {
                    var matchTypeResult = await _wordMatchingService.MatchWordAgainstNPM(pair.Key);

                    Assert.That(matchTypeResult, Is.EqualTo(pair.Value));
                });

            await Task.WhenAll(matchingTasks);
        }
    }
}