using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.NPM;

namespace NPMGame.Core.Engine.Words
{
    public interface IWordMatchingService
    {
        Task<MatchType> MatchWordAgainstNPM(string word);
    }

    public class WordMatchingService : IWordMatchingService
    {
        private const string _npmRegistryUrl = "https://registry.npmjs.org";

        private readonly IHttpClientFactory _httpClientFactory;

        public WordMatchingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MatchType> MatchWordAgainstNPM(string word)
        {
            var matchingNames = await GetMatchingNames(word);

            return GetTypeOfMatch(word, matchingNames.ToList());
        }

        private MatchType GetTypeOfMatch(string word, IReadOnlyCollection<string> packageNames)
        {
            var hasAnyExactMatches = packageNames
                .Any(package => string.Equals(word, package, StringComparison.CurrentCultureIgnoreCase));

            if (hasAnyExactMatches)
            {
                return MatchType.Exact;
            }

            var hasAnyPartialMatches = packageNames
                .Any(package => package.Contains(word, StringComparison.CurrentCultureIgnoreCase));

            if (hasAnyPartialMatches)
            {
                return MatchType.Partial;
            }

            return MatchType.None;
        }

        private async Task<IEnumerable<string>> GetMatchingNames(string word)
        {
            var searchResponse = await SearchForPackages(word);

            var matchingNames = searchResponse.objects
                .Select(o => o.package.name)
                .Where(name => name.Contains(word));

            return matchingNames;
        }

        private async Task<NPMSearchResponse> SearchForPackages(string word)
        {
            var searchApiUrl = $"{_npmRegistryUrl}/-/v1/search";

            var uriBuilder = new UriBuilder(searchApiUrl);

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["text"] = word;
            parameters["size"] = "250"; // Top 250 search items is good enough

            // Other potentially useful query params
            // parameters["from"] = ...;
            // parameters["quality"] = ...;
            // parameters["popularity"] = ...;
            // parameters["maintenance"] = ...;

            uriBuilder.Query = parameters.ToString();

            var requestUrl = uriBuilder.ToString();

            using (var client = _httpClientFactory.CreateClient())
            using (var response = await client.GetAsync(requestUrl))
            using (var content = response.Content)
            {
                var jsonResponse = await content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<NPMSearchResponse>(jsonResponse);

                return searchResponse;
            }
        }
    }
}
