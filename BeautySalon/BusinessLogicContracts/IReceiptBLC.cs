using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogicContracts;

public interface IReceiptBLC
{
    List<ReceiptDM> GetAllReceipts(bool onlyActive = true);
    List<ReceiptDM> GetFilteredReceipts(
        string? staffID = null,
        string? customerID = null,
        bool? isCanceled = null,
        DateTime? fromDateIssued = null,
        DateTime? toDateIssued = null);
    ReceiptDM GetReceiptById(string id);
    ReceiptDM GetReceiptWithItems(string id);
    void InsertReceipt(ReceiptDM receiptDataModel, string cashBoxId);
    void UpdateReceipt(ReceiptDM receiptDataModel);
    void DeleteReceipt(string id);
}