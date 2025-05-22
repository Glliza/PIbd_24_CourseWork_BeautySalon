using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class CustomerBLC : ICustomerBLC
{
    private readonly ICustomerSC _customerStorageContract;
    private readonly ILogger _logger;

    public CustomerBLC(ICustomerSC customerStorageContract, ILogger logger)
    {
        _customerStorageContract = customerStorageContract;
        _logger = logger;
    }

    public List<CustomerDM> GetAllCustomers(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllCustomers params: {onlyActive}", onlyActive);
        return _customerStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<CustomerDM> GetFilteredCustomers(string? fio = null, string? phoneNumber = null, DateTime? fromBirthDate = null, DateTime? toBirthDate = null)
    {
        _logger.LogInformation("GetFilteredCustomers params: {fio}, {phoneNumber}, {fromBirthDate}, {toBirthDate}", fio, phoneNumber, fromBirthDate, toBirthDate);
        return _customerStorageContract.GetList(onlyActive: true, fio, phoneNumber, fromBirthDate, toBirthDate).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public CustomerDM GetCustomerById(string id)
    {
        _logger.LogInformation("GetCustomerById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Customer ID is not a valid GUID");
        }

        var result = _customerStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public CustomerDM? GetCustomerByPhoneNumber(string phoneNumber)
    {
        _logger.LogInformation("GetCustomerByPhoneNumber for {phoneNumber}", phoneNumber);
        if (string.IsNullOrEmpty(phoneNumber))
        {
            throw new ArgumentNullException(nameof(phoneNumber));
        }
        var result = _customerStorageContract.GetElementByPhoneNumber(phoneNumber).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(phoneNumber);
    }

    public void InsertCustomer(CustomerDM customerDataModel)
    {
        _logger.LogInformation("New customer data: {json}", JsonSerializer.Serialize(customerDataModel));
        ArgumentNullException.ThrowIfNull(customerDataModel);
        customerDataModel.Validate();
        _customerStorageContract.AddElement(customerDataModel).GetAwaiter().GetResult();
    }

    public void UpdateCustomer(CustomerDM customerDataModel)
    {
        _logger.LogInformation("Update customer data: {json}", JsonSerializer.Serialize(customerDataModel));
        ArgumentNullException.ThrowIfNull(customerDataModel);
        customerDataModel.Validate();
        _customerStorageContract.UpdElement(customerDataModel).GetAwaiter().GetResult();
    }

    public void DeleteCustomer(string id)
    {
        _logger.LogInformation("Delete customer by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Customer ID is not a valid GUID");
        }
        _customerStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}