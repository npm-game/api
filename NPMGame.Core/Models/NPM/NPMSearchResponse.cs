using System;
using System.Collections.Generic;

namespace NPMGame.Core.Models.NPM
{
    public class NPMSearchResponse
    {
        public List<NPMSearchObject> objects { get; set; }
        public int total { get; set; }
        public string time { get; set; }
    }

    public class NPMSearchObject
    {
        public NPMSearchObjectPackage package { get; set; }
        public NPMSearchObjectScore score { get; set; }
        public double searchScore { get; set; }
    }

    public class NPMSearchObjectPackage
    {
        public string name { get; set; }
        public string version { get; set; }
        public string description { get; set; }

        public List<string> keywords { get; set; }
        public DateTime date { get; set; }

        public Dictionary<string, string> links { get; set; }
        public Dictionary<string, string> publisher { get; set; }
        public List<Dictionary<string, string>> maintainers { get; set; }
    }

    public class NPMSearchObjectScore
    {
        public float final { get; set; }
        public NPMSearchObjectScoreDetail detail { get; set; }
    }

    public class NPMSearchObjectScoreDetail
    {
        public float quality { get; set; }
        public float popularity { get; set; }
        public float maintenance { get; set; }
    }
}