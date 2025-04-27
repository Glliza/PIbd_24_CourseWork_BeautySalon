using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface ICustomerSC
{
    Task<List<CustomerDM>> GetList(
        bool onlyActive = true,
        string? fio = null,
        string? phoneNumber = null,
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null);

    Task<CustomerDM?> GetElementByID(string id);
    Task<CustomerDM?> GetElementByPhoneNumber(string phoneNumber);
    Task AddElement(CustomerDM customerDataModel);
    Task UpdElement(CustomerDM customerDataModel);
    Task DelElement(string id);
    Task RestoreElement(string id);
}
