using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class CashBoxBLC : ICashBoxBLC
{
    private readonly ICashBoxSC _cashBoxStorageContract;
    private readonly ILogger _logger;

    public CashBoxBLC(ICashBoxSC cashBoxStorageContract, ILogger logger)
    {
        _cashBoxStorageContract = cashBoxStorageContract;
        _logger = logger;
    }

    public List<CashBoxDM> GetAllCashBoxes(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllCashBoxes params: {onlyActive}", onlyActive);
        return _cashBoxStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public CashBoxDM GetCashBoxById(string id)
    {
        _logger.LogInformation("GetCashBoxById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("CashBox ID is not a valid GUID");
        }

        var result = _cashBoxStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public void InsertCashBox(CashBoxDM cashBoxDataModel)
    {
        _logger.LogInformation("New cashbox data: {json}", JsonSerializer.Serialize(cashBoxDataModel));
        ArgumentNullException.ThrowIfNull(cashBoxDataModel);
        cashBoxDataModel.Validate();
        _cashBoxStorageContract.AddElement(cashBoxDataModel).GetAwaiter().GetResult();
    }

    public void UpdateCashBox(CashBoxDM cashBoxDataModel)
    {
        _logger.LogInformation("Update cashbox data: {json}", JsonSerializer.Serialize(cashBoxDataModel));
        ArgumentNullException.ThrowIfNull(cashBoxDataModel);
        cashBoxDataModel.Validate();
        _cashBoxStorageContract.UpdElement(cashBoxDataModel).GetAwaiter().GetResult();
    }

    public void DeleteCashBox(string id)
    {
        _logger.LogInformation("Delete cashbox by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("CashBox ID is not a valid GUID");
        }
        _cashBoxStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}