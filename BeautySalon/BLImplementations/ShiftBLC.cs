using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class ShiftBLC : IShiftBLC
{
    private readonly IShiftSC _shiftStorageContract;
    private readonly ILogger _logger;

    public ShiftBLC(IShiftSC shiftStorageContract, ILogger logger)
    {
        _shiftStorageContract = shiftStorageContract;
        _logger = logger;
    }

    public List<ShiftDM> GetAllShifts(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllShifts params: {onlyActive}", onlyActive);
        return _shiftStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<ShiftDM> GetFilteredShifts(string? staffID = null, string? cashBoxID = null, DateTime? fromDateTimeStart = null, DateTime? toDateTimeStart = null, DateTime? fromDateTimeFinish = null, DateTime? toDateTimeFinish = null)
    {
        _logger.LogInformation("GetFilteredShifts params: {staffID}, {cashBoxID}, {fromDateTimeStart}, {toDateTimeStart}, {fromDateTimeFinish}, {toDateTimeFinish}", staffID, cashBoxID, fromDateTimeStart, toDateTimeStart, fromDateTimeFinish, toDateTimeFinish);
        return _shiftStorageContract.GetList(onlyActive: true, staffID, cashBoxID, fromDateTimeStart, toDateTimeStart, fromDateTimeFinish, toDateTimeFinish).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public ShiftDM GetShiftById(string id)
    {
        _logger.LogInformation("GetShiftById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Shift ID is not a valid GUID");
        }

        var result = _shiftStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public ShiftDM? GetActiveShiftForStaff(string staffID)
    {
        _logger.LogInformation("GetActiveShiftForStaff for {staffID}", staffID);
        if (string.IsNullOrEmpty(staffID))
        {
            throw new ArgumentNullException(nameof(staffID));
        }

        var result = _shiftStorageContract.GetActiveShiftForStaffAsync(staffID).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(staffID);
    }

    public void InsertShift(ShiftDM shiftDataModel)
    {
        _logger.LogInformation("New shift data: {json}", JsonSerializer.Serialize(shiftDataModel));
        ArgumentNullException.ThrowIfNull(shiftDataModel);
        shiftDataModel.Validate();
        _shiftStorageContract.AddElement(shiftDataModel).GetAwaiter().GetResult();
    }

    public void UpdateShift(ShiftDM shiftDataModel)
    {
        _logger.LogInformation("Update shift data: {json}", JsonSerializer.Serialize(shiftDataModel));
        ArgumentNullException.ThrowIfNull(shiftDataModel);
        shiftDataModel.Validate();
        _shiftStorageContract.UpdElement(shiftDataModel).GetAwaiter().GetResult();
    }

    public void DeleteShift(string id)
    {
        _logger.LogInformation("Delete shift by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Shift ID is not a valid GUID");
        }
        _shiftStorageContract.DelElement(id).GetAwaiter().GetResult();
    }

    public void EndShift(string shiftId, DateTime dateTimeFinish)
    {
        _logger.LogInformation("End shift {shiftId} at {dateTimeFinish}", shiftId, dateTimeFinish);
        if (string.IsNullOrEmpty(shiftId))
        {
            throw new ArgumentNullException(nameof(shiftId));
        }
        if (!shiftId.IsGuid())
        {
            throw new ValidationException("Shift ID is not a valid GUID");
        }

        _shiftStorageContract.EndShiftAsync(shiftId, dateTimeFinish).GetAwaiter().GetResult();
    }

    public void RestoreShift(string id)
    {
        _logger.LogInformation("Restore shift {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Shift ID is not a valid GUID");
        }
        _shiftStorageContract.RestoreElement(id).GetAwaiter().GetResult();
    }
}