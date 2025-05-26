using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.Entities;
using BeautySalon.Enums;
using System.Text.Json;

using BeautySalon.MailWork;
using BeautySalon.Contracts.BindingModels;

namespace BeautySalon.BLImplementations;

public class StaffBLC : IStaffBLC
{
    private readonly IStaffSC _staffStorage;
    private readonly MailKit _mailWorker; // Add this
    private readonly ILogger<StaffBLC> _logger;
    private readonly ReportGenerator _reportGenerator;

    public StaffBLC(IStaffSC staffStorage, MailKit mailWorker, ILogger<StaffBLC> logger, ReportGenerator reportGenerator)
    {
        _staffStorage = staffStorage;
        _mailWorker = mailWorker; // Add this
        _logger = logger;
        _reportGenerator = reportGenerator;
    }

    public async Task<List<Staff>> GetAllStaffAsync()
    {
        try
        {
            var staffList = _staffStorage.GetList();
            _logger.LogInformation("Successfully retrieved all staff.");

            // Generate and send report
            var reportFilePath = await _reportGenerator.GenerateStaffReport(staffList);

            await _mailWorker.MailSendAsync(new MailSendInfoBM // Create DTO to transport data
            {
                MailAddress = "test@example.com",
                Subject = "Staff Report",
                Text = "Please find attached the staff report."
            });
            return staffList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving all staff.");
            throw;
        }
    }

    public List<StaffDM> GetAllStaff(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllStaff params: {onlyActive}", onlyActive);
        return _staffStorage.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<StaffDM> GetFilteredStaff(DateTime? fromBirthDate = null, DateTime? toBirthDate = null, DateTime? fromEmploymentDate = null, DateTime? toEmploymentDate = null, PostType? postType = null)
    {
        _logger.LogInformation("GetFilteredStaff params: {fromBirthDate}, {toBirthDate}, {fromEmploymentDate}, {toEmploymentDate}, {postType}", fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType);
        return _staffStorage.GetList(onlyActive: true, fromBirthDate, toBirthDate, fromEmploymentDate, toEmploymentDate, postType).GetAwaiter().GetResult()
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

        var result = _staffStorage.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public StaffDM GetStaffByFIO(string fio)
    {
        _logger.LogInformation("GetStaffByFIO for {fio}", fio);
        if (string.IsNullOrEmpty(fio))
        {
            throw new ArgumentNullException(nameof(fio));
        }
        var result = _staffStorage.GetElementByFIO(fio).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(fio);
    }

    public void InsertStaff(StaffDM staffDataModel)
    {
        _logger.LogInformation("New staff data: {json}", JsonSerializer.Serialize(staffDataModel));
        ArgumentNullException.ThrowIfNull(staffDataModel);
        staffDataModel.Validate();
        _staffStorage.AddElement(staffDataModel).GetAwaiter().GetResult();
    }

    public void UpdateStaff(StaffDM staffDataModel)
    {
        _logger.LogInformation("Update staff data: {json}", JsonSerializer.Serialize(staffDataModel));
        ArgumentNullException.ThrowIfNull(staffDataModel);
        staffDataModel.Validate();
        _staffStorage.UpdElement(staffDataModel).GetAwaiter().GetResult();
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
        _staffStorage.DelElement(id).GetAwaiter().GetResult();
    }
}