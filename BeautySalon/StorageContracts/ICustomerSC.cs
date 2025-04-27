using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface ICustomerSC
{
    // Get a list of customers with optional filtering
    // Note: Filters align with potential Customer properties (FIO, BirthDate, PhoneNumber)
    Task<List<CustomerDM>> GetList(
        bool onlyActive = true,
        string? fio = null,
        string? phoneNumber = null,
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null);

    Task<CustomerDM?> GetElementByID(string id);

    // Get a single customer by their phone number (assuming it's unique or you want the first match)
    Task<CustomerDM?> GetElementByPhoneNumber(string phoneNumber); // Changed from GetElementByFIO as Phone is often a primary lookup

    Task AddElement(CustomerDM customerDataModel);

    Task UpdElement(CustomerDM customerDataModel);

    Task DelElement(string id);

    Task RestoreElement(string id);
}
