using System;
using NPMGame.Core.Models.Enums;

namespace NPMGame.Core.Words
{
    public static class WordMatcher
    {
        public static MatchType MatchWordWithPackage(string word, string packageName)
        {
            if (string.Equals(word, packageName, StringComparison.CurrentCultureIgnoreCase))
            {
                return MatchType.Exact;
            }

            if (packageName.Contains(word, StringComparison.CurrentCultureIgnoreCase))
            {
                return MatchType.Partial;
            }

            return MatchType.None;
        }
    }
}
