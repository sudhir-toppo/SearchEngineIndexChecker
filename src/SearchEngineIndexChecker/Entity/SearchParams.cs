using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngineIndexChecker.Entity
{
    public class SearchParams
    {
        public string SearchKeyword { get; set; }
        public string KeywordForOccurenceCount { get; set; }
        public int NumberOfResultsToConsider { get; set; }
        public bool AllowReadingFromCache { get; set; }
    }
}
