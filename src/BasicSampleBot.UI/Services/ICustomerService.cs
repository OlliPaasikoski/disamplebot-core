namespace BasicSampleBot.Services
{
    using BasicSampleBot.SQLCore;
    public interface ICustomerService
    {
        Customer GetCustomerByName(string firstName, string lastName);
    }
}
