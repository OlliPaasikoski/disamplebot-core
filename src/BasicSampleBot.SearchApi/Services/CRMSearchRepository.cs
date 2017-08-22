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
    public class CRMSearchRepository : ICRMSearchRepository
    {
        // should be read-only
        SearchIndexClient _searchClient = null;
        const string CRMIndexName = "crmindex";

        public CRMSearchRepository()
        {
            // The ctor depends on config values, the service could be dependency injected but who knows
            _searchClient = new SearchIndexClient(
                ConfigurationManager.AppSettings["SearchServiceName"],
                CRMIndexName,
                new SearchCredentials(ConfigurationManager.AppSettings["SearchServiceQueryApiKey"]));
        }

        public async Task<DocumentSearchResult<CRMSearchEntity>> BasicSearch(string term)
        {
            SearchParameters parameters;
            DocumentSearchResult<CRMSearchEntity> results;

            parameters = new SearchParameters();

            results = await _searchClient.Documents.SearchAsync<CRMSearchEntity>(term);
            foreach (SearchResult<CRMSearchEntity> result in results.Results)
            {
                Trace.WriteLine(result.Document);
            }
            return results;
        }

    }
}
