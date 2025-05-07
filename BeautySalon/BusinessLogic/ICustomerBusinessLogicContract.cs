using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogic;

public interface ICustomerBusinessLogicContract
{
    List<CustomerDM> GetAllCustomers(bool onlyActive = true);

    List<CustomerDM> GetFilteredCustomers(
        string? fio = null,
        string? phoneNumber = null,
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null);

    CustomerDM GetCustomerById(string id);
    CustomerDM GetCustomerByPhoneNumber(string phoneNumber);
    void InsertCustomer(CustomerDM customerDataModel);
    void UpdateCustomer(CustomerDM customerDataModel);
    void DeleteCustomer(string id);
}