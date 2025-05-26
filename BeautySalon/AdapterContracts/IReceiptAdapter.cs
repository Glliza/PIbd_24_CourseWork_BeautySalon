using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IReceiptAdapter
{
    ReceiptR GetReceiptById(string id);
    ReceiptR CreateReceipt(ReceiptVM receiptModel);
    ReceiptR UpdateReceipt(ReceiptVM receiptModel);
    ReceiptR DeleteReceipt(string id);
    ReceiptR GetAllReceipts(bool onlyActive = true);
}