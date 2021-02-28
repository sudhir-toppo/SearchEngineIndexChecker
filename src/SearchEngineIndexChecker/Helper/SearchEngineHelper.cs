using Microsoft.Extensions.Configuration;
using SearchEngineIndexChecker.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchEngineIndexChecker.Helper
{
    public interface ISearchEngineHelper
    {
        Dictionary<string, List<int>> ExecuteSearch(SearchParams searchParams);
    }
    public class SearchEngineHelper : ISearchEngineHelper
    {
        private ICacheHelper _cacheHelper;
        private IConfigurationRoot _config;

        public SearchEngineHelper(IConfigurationRoot config)
        {
            _config = config;
            _cacheHelper = new CacheHelper();
        }

        private List<SearchEngineConfiguration> GetSearchEngineList()
        {
            var searchEngineList = _config.Get<SearchEngineConfigurationCollection>();
            return searchEngineList.SearchEngines;
        }

        private List<int> ProcessQuery(SearchEngineConfiguration searchEngine, SearchParams searchParams, out bool isFromCache)
        {
            var indexStats = _cacheHelper.Get<List<int>>(searchEngine.Name);
            isFromCache = true;
            if (null == indexStats || !searchParams.AllowReadingFromCache)
            {
                isFromCache = false;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(searchEngine.Url, searchParams.SearchKeyword, searchParams.NumberOfResultsToConsider));
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.ASCII);
                string rawResponse = streamReader.ReadToEnd();
                var resultList = Regex.Split(rawResponse, searchEngine.UniqueResultSplitter).Skip(1).Select((a, i) => new { Index = i + 1, Url = a.Split(new Char[] { ' ', '<' })[0] }).ToList();
                indexStats = resultList.Where(a => a.Url.ToLower().Contains(searchParams.KeywordForOccurenceCount.ToLower())).Select(a => a.Index).ToList();

                _cacheHelper.Add(searchEngine.Name, indexStats);
            }
            return indexStats;
        }

        public Dictionary<string, List<int>> ExecuteSearch(SearchParams searchParams)
        {
            var searchEngineList = GetSearchEngineList();
            var results = new Dictionary<string, List<int>>();

            Parallel.ForEach(searchEngineList, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, (a) =>
               {
                   var entry = new List<int>();
                   bool isFromCache = false;

                   try
                   {
                       entry = ProcessQuery(a, searchParams, out isFromCache);
                   }
                   catch (Exception exception)
                   {
                       //TODO: handle/log exception
                   }
                   results.Add(a.Name + (isFromCache ? " (cached)" : string.Empty), entry);
               });

            return results;
        }
    }
}
