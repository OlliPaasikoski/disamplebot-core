using BasicSampleBot.BusinessCore;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSampleBot.SearchApi.Services
{
    public static class SearchIndexes
    {
        public const string Customers = "customerindex";
        public const string Products = "productindex";
    }

    public class CRMSearchRepository<T> : ICRMSearchRepository where T : SearchEntity
    {
        // should be read-only
        SearchIndexClient _searchClient = null;

        public CRMSearchRepository()
        {
            var indexName = (typeof(T) == typeof(CustomerEntity)) ? SearchIndexes.Customers : SearchIndexes.Products;

            // The ctor depends on config values, the service could be dependency injected but who knows
            _searchClient = new SearchIndexClient(
                ConfigurationManager.AppSettings["SearchServiceName"],
                indexName,
                new SearchCredentials(ConfigurationManager.AppSettings["SearchServiceQueryApiKey"]));
        }

        public async Task<DocumentSearchResult<T>> Search(string term)
        {
            SearchParameters parameters = new SearchParameters(); // not used
            return await _searchClient.Documents.SearchAsync<T>(term);
        }

    }
}
