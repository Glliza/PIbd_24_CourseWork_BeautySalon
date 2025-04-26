using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetCustomerByPhoneNumber(string phoneNumber);
    Task<IEnumerable<Customer>> FindCustomersByName(string searchName);
}
