using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogicContracts;
public interface IShiftBLC
{
    List<ShiftDM> GetAllShifts(bool onlyActive = true);
    List<ShiftDM> GetFilteredShifts(
        string? staffID = null,
        string? cashBoxID = null,
        DateTime? fromDateTimeStart = null,
        DateTime? toDateTimeStart = null,
        DateTime? fromDateTimeFinish = null,
        DateTime? toDateTimeFinish = null);
    ShiftDM GetShiftById(string id);
    ShiftDM? GetActiveShiftForStaff(string staffID);
    void InsertShift(ShiftDM shiftDataModel);
    void UpdateShift(ShiftDM shiftDataModel);
    void DeleteShift(string id);
    void EndShift(string shiftId, DateTime dateTimeFinish);
    void RestoreShift(string id);
}