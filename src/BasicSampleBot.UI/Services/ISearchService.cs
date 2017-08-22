namespace BasicSampleBot.UI
{
    using AutoMapper;
    using BasicSampleBot.BusinessCore;
    using BasicSampleBot.SearchApi.Services;
    using Microsoft.Azure.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SearchService : ISearchService
    {
        private CRMSearchRepository _crmSearchRepository;

        public SearchService()
        {
            _crmSearchRepository = new CRMSearchRepository();
            AutomapperConfiguration.Configure();
        }

        public async Task<IEnumerable<CRMSearchDto>> GetSearchResults(string searchTerm)
        {
            var searchResults = await _crmSearchRepository.BasicSearch(searchTerm);
            IList<CRMSearchDto> retval = new List<CRMSearchDto>();

            foreach (SearchResult<CRMSearchEntity> result in searchResults.Results)
            {
                retval.Add(Mapper.Map<CRMSearchDto>(result.Document));
            }

            return retval;
        }
    }

    public interface ISearchService
    {
        Task<IEnumerable<CRMSearchDto>> GetSearchResults(string searchTerm);
    }
}
