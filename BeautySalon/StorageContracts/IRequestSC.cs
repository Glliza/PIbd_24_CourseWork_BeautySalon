using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.StorageContracts;

public interface IRequestSC
{
    Task<List<RequestDM>> GetList(
        bool onlyActive = true,
        string? customerID = null,
        OrderStatus? status = null,
        DateTime? fromDateCreated = null,
        DateTime? toDateCreated = null);

    Task<RequestDM?> GetElementByID(string id);
    Task<RequestDM?> GetRequestWithItemsAsync(string id);
    Task AddElement(RequestDM requestDataModel);
    Task UpdElement(RequestDM requestDataModel);
    Task DelElement(string id);
}
