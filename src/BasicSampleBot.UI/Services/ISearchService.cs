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
        CRMSearchRepository<CustomerEntity> _customerSearchRepository;
        CRMSearchRepository<ProductEntity> _productSearchRepository;

        public SearchService()
        {
            _customerSearchRepository = new CRMSearchRepository<CustomerEntity>();
            _productSearchRepository = new CRMSearchRepository<ProductEntity>();
            AutomapperConfiguration.Configure();
        }

        public async Task<IEnumerable<CustomerSearchDto>> CustomerLookup(string searchTerm)
        {
            var searchResults = await _customerSearchRepository.Search(searchTerm);
            IList<CustomerSearchDto> retval = new List<CustomerSearchDto>();

            foreach (SearchResult<CustomerEntity> result in searchResults.Results)
            {
                if (result.Score > 1.5)
                {
                    retval.Add(Mapper.Map<CustomerSearchDto>(result.Document));
                }           
            }

            return retval;
        }

        public async Task<IEnumerable<ProductSearchDto>> ProductLookup(string searchTerm)
        {
            var searchResults = await _productSearchRepository.Search(searchTerm);
            IList<ProductSearchDto> retval = new List<ProductSearchDto>();

            foreach (SearchResult<ProductEntity> result in searchResults.Results)
            {
                if (result.Score > 1.5)
                {
                    retval.Add(Mapper.Map<ProductSearchDto>(result.Document));
                }          
            }

            return retval;
        }
    }

    public interface ISearchService
    {
        Task<IEnumerable<CustomerSearchDto>> CustomerLookup(string searchTerm);
        Task<IEnumerable<ProductSearchDto>> ProductLookup(string searchTerm);
    }
}
