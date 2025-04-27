using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.StorageContracts;

public interface IRequestSC
{
    // Get a list of requests with optional filtering
    // Filters align with potential Request properties (Customer, Date range, Status)
    Task<List<RequestDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag
        string? requestID = null, // Filter by specific Request ID
        string? customerID = null,
        OrderStatus? status = null, // Filter by Status enum
        DateTime? fromDateCreated = null,
        DateTime? toDateCreated = null);

    Task<RequestDM?> GetElementByID(string id);

    // Get a single request by its unique ID, including its list items
    Task<RequestDM?> GetRequestWithItemsAsync(string id);

    // Add a new request (and its list items)
    Task AddElement(RequestDM requestDataModel);

    Task UpdElement(RequestDM requestDataModel);

    Task DelElement(string id);

    // Restore a soft-deleted request by ID (optional but good practice)
    Task RestoreElement(string id);

    // Optional: Methods to modify list items (add, remove) if needed separately
    // Task AddItemsToRequestAsync(string requestId, IEnumerable<ProductListItemDM> productItems, IEnumerable<ServiceListItemDM> serviceItems);
    // Task RemoveItemFromRequestAsync(string requestId, string listItemId);
}
