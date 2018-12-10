using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.NPM;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace NPMGame.Core.Tests.Engine.Words
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
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient().Returns(x =>
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

                return mockHttp.ToHttpClient();
            });

            _wordMatchingService = new WordMatchingService(httpClientFactory);
        }

        [Test]
        [TestCase("axios", MatchType.Exact)]
        [TestCase("crikey", MatchType.Exact)]
        [TestCase("hello", MatchType.Exact)]
        [TestCase("axio", MatchType.Partial)]
        [TestCase("faceboo", MatchType.Partial)]
        [TestCase("darkly", MatchType.Partial)]
        [TestCase("vixens", MatchType.None)]
        [TestCase("axiosify", MatchType.None)]
        [TestCase("tangleberry", MatchType.None)]
        public async Task TestWordMatchingAgainstNPM(string word, MatchType matchType)
        {
            var matchTypeResult = await _wordMatchingService.MatchWordAgainstNPM(word);

            Assert.That(matchTypeResult, Is.EqualTo(matchType));
        }
    }
}