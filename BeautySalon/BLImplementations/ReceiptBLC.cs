using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class ReceiptBLC : IReceiptBLC
{
    private readonly IReceiptSC _receiptStorageContract;
    private readonly ILogger _logger;

    public ReceiptBLC(IReceiptSC receiptStorageContract, ILogger logger)
    {
        _receiptStorageContract = receiptStorageContract;
        _logger = logger;
    }

    public List<ReceiptDM> GetAllReceipts(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllReceipts params: {onlyActive}", onlyActive);
        return _receiptStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<ReceiptDM> GetFilteredReceipts(string? staffID = null, string? customerID = null, bool? isCanceled = null, DateTime? fromDateIssued = null, DateTime? toDateIssued = null)
    {
        _logger.LogInformation("GetFilteredReceipts params: {staffID}, {customerID}, {isCanceled}, {fromDateIssued}, {toDateIssued}", staffID, customerID, isCanceled, fromDateIssued, toDateIssued);
        return _receiptStorageContract.GetList(onlyActive: true, staffID, customerID, isCanceled, fromDateIssued, toDateIssued).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public ReceiptDM GetReceiptById(string id)
    {
        _logger.LogInformation("GetReceiptById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Receipt ID is not a valid GUID");
        }

        var result = _receiptStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public ReceiptDM GetReceiptWithItems(string id)
    {
        _logger.LogInformation("GetReceiptWithItems for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Receipt ID is not a valid GUID");
        }

        var result = _receiptStorageContract.GetReceiptWithItemsAsync(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public void InsertReceipt(ReceiptDM receiptDataModel, string cashBoxId)
    {
        _logger.LogInformation("New receipt data: {json} for cashbox {cashBoxId}", JsonSerializer.Serialize(receiptDataModel), cashBoxId);
        ArgumentNullException.ThrowIfNull(receiptDataModel);
        receiptDataModel.Validate();
        _receiptStorageContract.AddElement(receiptDataModel, cashBoxId).GetAwaiter().GetResult();
    }

    public void UpdateReceipt(ReceiptDM receiptDataModel)
    {
        _logger.LogInformation("Update receipt data: {json}", JsonSerializer.Serialize(receiptDataModel));
        ArgumentNullException.ThrowIfNull(receiptDataModel);
        receiptDataModel.Validate();
        _receiptStorageContract.UpdElement(receiptDataModel).GetAwaiter().GetResult();
    }

    public void DeleteReceipt(string id)
    {
        _logger.LogInformation("Delete receipt by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Receipt ID is not a valid GUID");
        }
        _receiptStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}