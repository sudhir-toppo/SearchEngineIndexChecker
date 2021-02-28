using Microsoft.Extensions.Configuration;
using SearchEngineIndexChecker.Entity;
using SearchEngineIndexChecker.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchEngineIndexChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Initialize();
            StartProcess(config);
        }

        private static void StartProcess(IConfigurationRoot config)
        {
            SearchParams searchParams = GetUserInput();
            do
            {
                Console.WriteLine("Searching...");
                var results = ExecuteSearch(config, searchParams);
                Display(results);

                Console.WriteLine();
                Console.WriteLine("Press 1 to force hit search engines");
                Console.WriteLine("Press any other key to check again (may read from cache)");
                var option = Console.ReadLine();

                searchParams.AllowReadingFromCache = true;
                int temp;
                if (int.TryParse(option, out temp))
                {
                    searchParams.AllowReadingFromCache = (temp != 1);
                }
            }
            while (true);
        }

        private static SearchParams GetUserInput()
        {
            SearchParams searchParams = new SearchParams();

            Console.Write("Search keyword: ");
            searchParams.SearchKeyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(searchParams.SearchKeyword))
            {
                searchParams.SearchKeyword = "e-settlement";
                Console.WriteLine(searchParams.SearchKeyword);
            }

            Console.Write("Keyword for occurence count: ");
            searchParams.KeywordForOccurenceCount = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(searchParams.KeywordForOccurenceCount))
            {
                searchParams.KeywordForOccurenceCount = "www.sympli.com.au";
                Console.WriteLine(searchParams.KeywordForOccurenceCount);
            }

            searchParams.NumberOfResultsToConsider = 100;

            return searchParams;
        }

        private static void Display(Dictionary<string, List<int>> searchResults)
        {
            Console.Clear();
            Console.WriteLine(DateTime.Now.ToLongTimeString());

            foreach (var result in searchResults.OrderBy(a => a.Key))
            {
                Console.WriteLine($"{"\t"}{result.Key}: {(result.Value.Count == 0 ? "Error" : string.Join(", ", result.Value))}");
            }
        }

        private static Dictionary<string, List<int>> ExecuteSearch(IConfigurationRoot config, SearchParams searchParams)
        {
            return new SearchEngineHelper(config).ExecuteSearch(searchParams);
        }

        private static IConfigurationRoot Initialize()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build();
        }
    }
}
