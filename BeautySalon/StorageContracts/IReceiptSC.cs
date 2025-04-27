using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IReceiptSC
{
    Task<List<ReceiptDM>> GetList(
        bool onlyActive = true,
        string? staffID = null,
        string? customerID = null,
        bool? isCanceled = null,
        DateTime? fromDateIssued = null,
        DateTime? toDateIssued = null);

    Task<ReceiptDM?> GetElementByID(string id);
    Task<ReceiptDM?> GetReceiptWithItemsAsync(string id);
    Task AddElement(ReceiptDM receiptDataModel, string cashBoxId); 
    Task UpdElement(ReceiptDM receiptDataModel);
    Task DelElement(string id);
}
