using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngineIndexChecker.Entity
{
    public class SearchEngineConfiguration
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string UniqueResultSplitter { get; set; }
    }

    public class SearchEngineConfigurationCollection
    {
        public List<SearchEngineConfiguration> SearchEngines { get; set; }
    }

}
