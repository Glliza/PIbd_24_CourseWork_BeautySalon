using System.Text.Json;
using BeautySalon.BusinessLogic;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BeautySalon.BLImplementations;


internal class CustomerBusinessLogicContract(ICustomerSC customerStorageContract, ILogger logger) : ICustomerBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly ICustomerSC _customerStorageContract = customerStorageContract;

    public List<CustomerDM> GetAllCustomers(bool onlyActive = true)
    {
        try
        {
            _logger.LogInformation("GetAllCustomers params: {onlyActive}", onlyActive);
            var result = _customerStorageContract.GetList(onlyActive).GetAwaiter().GetResult();
            return result ?? throw new NullListException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllCustomers");
            throw new StorageException(ex);
        }
    }

    public List<CustomerDM> GetFilteredCustomers(
        string? fio = null,
        string? phoneNumber = null,
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null)
    {
        try
        {
            _logger.LogInformation(
                "GetFilteredCustomers params: FIO: {fio}, Phone: {phoneNumber}, BirthDate: {fromBirthDate}-{toBirthDate}",
                fio, phoneNumber, fromBirthDate, toBirthDate);

            if (fromBirthDate.HasValue && toBirthDate.HasValue && fromBirthDate > toBirthDate)
            {
                throw new IncorrectDatesException(fromBirthDate.Value, toBirthDate.Value);
            }

            if (!string.IsNullOrEmpty(phoneNumber) && !IsValidPhoneNumber(phoneNumber))
            {
                throw new ValidationException("Номер телефона должен содержать 11 цифр");
            }

            var result = _customerStorageContract.GetList(true, fio, phoneNumber, fromBirthDate, toBirthDate)
                .GetAwaiter().GetResult();

            return result ?? throw new NullListException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFilteredCustomers");
            throw ex is ValidationException or IncorrectDatesException ? ex : new StorageException(ex);
        }
    }

    public CustomerDM GetCustomerById(string id)
    {
        try
        {
            _logger.LogInformation("GetCustomerById for {id}", id);

            if (id.IsEmpty()) throw new ArgumentNullException(nameof(id));
            if (!id.IsGuid()) throw new ValidationException("Идентификатор должен быть в формате GUID");

            var result = _customerStorageContract.GetElementByID(id).GetAwaiter().GetResult();
            return result ?? throw new ElementNotFoundException(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCustomerById");
            throw ex is ValidationException or ElementNotFoundException ? ex : new StorageException(ex);
        }
    }

    public CustomerDM GetCustomerByPhoneNumber(string phoneNumber)
    {
        try
        {
            _logger.LogInformation("GetCustomerByPhoneNumber for {phoneNumber}", phoneNumber);

            if (phoneNumber.IsEmpty()) throw new ArgumentNullException(nameof(phoneNumber));
            if (!IsValidPhoneNumber(phoneNumber)) throw new ValidationException("Неверный формат номера телефона");

            var result = _customerStorageContract.GetElementByPhoneNumber(phoneNumber).GetAwaiter().GetResult();
            return result ?? throw new ElementNotFoundException(phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCustomerByPhoneNumber");
            throw ex is ValidationException or ElementNotFoundException ? ex : new StorageException(ex);
        }
    }

    public void InsertCustomer(CustomerDM customerDataModel)
    {
        try
        {
            _logger.LogInformation("New customer data: {json}", JsonSerializer.Serialize(customerDataModel));

            ArgumentNullException.ThrowIfNull(customerDataModel);
            customerDataModel.Validate();

            if (!IsValidPhoneNumber(customerDataModel.PhoneNumber))
                throw new ValidationException("Номер телефона должен содержать 11 цифр");

            // Проверка на существование клиента
            var existingCustomer = _customerStorageContract
                .GetElementByPhoneNumber(customerDataModel.PhoneNumber)
                .GetAwaiter().GetResult();

            if (existingCustomer != null)
                throw new ElementExistsException("PhoneNumber", customerDataModel.PhoneNumber);

            _customerStorageContract.AddElement(customerDataModel).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InsertCustomer");
            throw ex is ValidationException or ElementExistsException ? ex : new StorageException(ex);
        }
    }

    public void UpdateCustomer(CustomerDM customerDataModel)
    {
        try
        {
            _logger.LogInformation("Update customer data: {json}", JsonSerializer.Serialize(customerDataModel));

            ArgumentNullException.ThrowIfNull(customerDataModel);
            customerDataModel.Validate();

            if (!IsValidPhoneNumber(customerDataModel.PhoneNumber))
                throw new ValidationException("Номер телефона должен содержать 11 цифр");

            var existingCustomer = _customerStorageContract
                .GetElementByID(customerDataModel.Id)
                .GetAwaiter().GetResult();

            if (existingCustomer == null)
                throw new ElementNotFoundException(customerDataModel.Id);

            _customerStorageContract.UpdElement(customerDataModel).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateCustomer");
            throw ex is ValidationException or ElementNotFoundException ? ex : new StorageException(ex);
        }
    }

    public void DeleteCustomer(string id)
    {
        try
        {
            _logger.LogInformation("Delete customer by id: {id}", id);

            if (id.IsEmpty()) throw new ArgumentNullException(nameof(id));
            if (!id.IsGuid()) throw new ValidationException("Идентификатор должен быть в формате GUID");

            var existingCustomer = _customerStorageContract
                .GetElementByID(id)
                .GetAwaiter().GetResult();

            if (existingCustomer == null)
                throw new ElementNotFoundException(id);

            _customerStorageContract.DelElement(id).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteCustomer");
            throw ex is ValidationException or ElementNotFoundException ? ex : new StorageException(ex);
        }
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.IsEmpty()) return false;
        // Удаляем все нецифровые символы
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        return digitsOnly.Length == 11; // Проверка для российских номеров
    }
}
