namespace BasicSampleBot.Services
{
    using BasicSampleBot.UI;
    using System.Linq;
    public static class ServiceFactory
    {
        //public static ICustomerService CustomerData = new CustomerService();
        public static ISearchService SearchData = new SearchService();
    }

    //public class CustomerService : ICustomerService
    //{
    //    private CustomerContext _context;
    //    public CustomerService()
    //    {
    //        _context = new CustomerContext();
    //    }

    //    public Customer GetCustomerByName(string firstName, string lastName)
    //    {
    //        return _context.Customers
    //                        .Where(c => c.FirstName.Equals(firstName) && c.LastName.Equals(lastName))
    //                        .FirstOrDefault();
    //    }
    //}
}
