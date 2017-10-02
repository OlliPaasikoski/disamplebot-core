namespace BasicSampleBot.UI.Services
{
    /// <summary>
    /// Kind of hack to enable serialization 
    /// while providing data access
    /// </summary>
    public static class ServiceFactory
    {
        public static ISearchService SearchData = new SearchService();
    }
}
