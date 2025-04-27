using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IShiftSC
{
    Task<List<ShiftDM>> GetList(
        bool onlyActive = true,
        string? staffID = null,
        string? cashBoxID = null,
        DateTime? fromDateTimeStart = null,
        DateTime? toDateTimeStart = null,
        DateTime? fromDateTimeFinish = null,
        DateTime? toDateTimeFinish = null);

    Task<ShiftDM?> GetElementByID(string id);
    Task<ShiftDM?> GetActiveShiftForStaffAsync(string staffID);
    Task AddElement(ShiftDM shiftDataModel);
    Task UpdElement(ShiftDM shiftDataModel);
    Task DelElement(string id);
    Task EndShiftAsync(string shiftId, DateTime dateTimeFinish);
}
