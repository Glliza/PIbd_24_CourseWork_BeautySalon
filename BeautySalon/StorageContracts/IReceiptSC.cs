using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IReceiptSC
{
    // Get a list of receipts with optional filtering
    // Filters align with potential Receipt properties (Staff, Customer, Date range, IsCanceled)
    Task<List<ReceiptDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag
        string? receiptID = null, // Filter by specific Receipt ID
        string? staffID = null,
        string? customerID = null,
        bool? isCanceled = null, // Filter by IsCanceled flag
        DateTime? fromDateIssued = null,
        DateTime? toDateIssued = null);

    Task<ReceiptDM?> GetElementByID(string id);

    // Get a single receipt by its unique ID, including its product items
    Task<ReceiptDM?> GetReceiptWithItemsAsync(string id);

    // Add a new receipt (and its product items)
    // Need to include CashBoxID when adding a receipt, as it's a required FK.
    Task AddElement(ReceiptDM receiptDataModel, string cashBoxId); // Added cashBoxId parameter

    // Update an existing receipt (and its product items)
    Task UpdElement(ReceiptDM receiptDataModel);

    Task DelElement(string id);

    Task RestoreElement(string id);

    // Optional: Method to get receipts for a specific visit
    Task<ReceiptDM?> GetReceiptByVisitIdAsync(string visitId);
}
