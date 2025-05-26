using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts
{
    public interface ICustomerAdapter
    {
        CustomerR GetCustomerById(string id);
        CustomerR GetCustomerByPhoneNumber(string phoneNumber);
        CustomerR RegisterCustomer(CustomerVM customerModel);
        CustomerR ChangeCustomerInfo(CustomerVM customerModel);
        CustomerR RemoveCustomer(string id);
        CustomerR GetAllCustomers(bool onlyActive = true);
        CustomerR GetCustomerByFIO(string fio);
    }
}
