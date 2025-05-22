using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.Enums;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class StaffBLC : IStaffBLC
{
    private readonly IStaffSC _staffStorageContract;
    private readonly ILogger _logger;

    public StaffBLC(IStaffSC staffStorageContract, ILogger logger)
    {
        _staffStorageContract = staffStorageContract;
        _logger = logger;
    }

    public List<StaffDM> GetAllStaff(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllStaff params: {onlyActive}", onlyActive);
        return _staffStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<StaffDM> GetFilteredStaff(DateTime? fromBirthDate = null, DateTime? toBirthDate = null, DateTime? fromEmploymentDate = null, DateTime? toEmploymentDate = null, PostType? postType = null)
    {
        _logger.LogInformation("GetFilteredStaff params: {fromBirthDate}, {toBirthDate}, {fromEmploymentDate}, {toEmploymentDate}, {postType}", fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType);
        return _staffStorageContract.GetList(onlyActive: true, fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public StaffDM GetStaffById(string id)
    {
        _logger.LogInformation("GetStaffById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Staff ID is not a valid GUID");
        }

        var result = _staffStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public StaffDM GetStaffByFIO(string fio)
    {
        _logger.LogInformation("GetStaffByFIO for {fio}", fio);
        if (string.IsNullOrEmpty(fio))
        {
            throw new ArgumentNullException(nameof(fio));
        }
        var result = _staffStorageContract.GetElementByFIO(fio).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(fio);
    }

    public void InsertStaff(StaffDM staffDataModel)
    {
        _logger.LogInformation("New staff data: {json}", JsonSerializer.Serialize(staffDataModel));
        ArgumentNullException.ThrowIfNull(staffDataModel);
        staffDataModel.Validate();
        _staffStorageContract.AddElement(staffDataModel).GetAwaiter().GetResult();
    }

    public void UpdateStaff(StaffDM staffDataModel)
    {
        _logger.LogInformation("Update staff data: {json}", JsonSerializer.Serialize(staffDataModel));
        ArgumentNullException.ThrowIfNull(staffDataModel);
        staffDataModel.Validate();
        _staffStorageContract.UpdElement(staffDataModel).GetAwaiter().GetResult();
    }

    public void DeleteStaff(string id)
    {
        _logger.LogInformation("Delete staff by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Staff ID is not a valid GUID");
        }
        _staffStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}