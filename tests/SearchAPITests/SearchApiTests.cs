using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSampleBot.SearchApi.Services;
using System.Threading.Tasks;

namespace SearchAPITests
{
    [TestClass]
    public class SearchApiTests
    {
        CRMSearchRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new CRMSearchRepository();
        }

        [TestMethod]
        public async Task SearchForOpenEntitiesReturnsSomethingOfValue()
        {
            string searchTerm = "Open";
            await _repo.BasicSearch(searchTerm);
        }
    }
}
