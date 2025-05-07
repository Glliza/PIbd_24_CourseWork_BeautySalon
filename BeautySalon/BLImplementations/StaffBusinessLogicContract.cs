using System.Text.Json;
using BeautySalon.BusinessLogic;
using BeautySalon.DataModels;
using BeautySalon.Enums;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BeautySalon.BLImplementations;

internal class StaffBusinessLogicContract(IStaffSC staffStorageContract, ILogger logger)
    : IStaffBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IStaffSC _staffStorageContract = staffStorageContract;

    public List<StaffDM> GetAllStaff(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllStaff params: {onlyActive}", onlyActive);
        return _staffStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
            ?? throw new NullListException();
    }

    public List<StaffDM> GetFilteredStaff(
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null,
        DateTime? fromEmploymentDate = null,
        DateTime? toEmploymentDate = null,
        PostType? postType = null)
    {
        _logger.LogInformation(
            "GetFilteredStaff params: BirthDate {fromBirthDate}-{toBirthDate}, " +
            "Employment {fromEmploymentDate}-{toEmploymentDate}, PostType: {postType}",
            fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType);

        // Валидация диапазонов дат
        if (fromBirthDate.HasValue && toBirthDate.HasValue && fromBirthDate > toBirthDate)
        {
            throw new ValidationException("Дата рождения 'от' не может быть позже даты 'до'");
        }

        if (fromEmploymentDate.HasValue && toEmploymentDate.HasValue &&
            fromEmploymentDate > toEmploymentDate)
        {
            throw new ValidationException("Дата приема 'от' не может быть позже даты 'до'");
        }

        var result = _staffStorageContract.GetList(
            true, fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType)
            .GetAwaiter().GetResult();

        return result ?? throw new NullListException();
    }

    public StaffDM GetStaffById(string id)
    {
        _logger.LogInformation("GetStaffById for {id}", id);

        if (id.IsEmpty())
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Идентификатор сотрудника не является GUID");
        }

        var result = _staffStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public StaffDM GetStaffByFIO(string fio)
    {
        _logger.LogInformation("GetStaffByFIO for {fio}", fio);

        if (fio.IsEmpty())
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

        if (id.IsEmpty())
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Идентификатор сотрудника не является GUID");
        }

        _staffStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}