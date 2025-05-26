using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IShiftAdapter
{
    ShiftR GetShiftById(string id);
    ShiftR GetActiveShiftForStaff(string staffID);
    ShiftR StartShift(ShiftVM shiftModel);
    ShiftR EndShift(string shiftId, DateTime dateTimeFinish);
    ShiftR UpdateShift(ShiftVM shiftModel);
    ShiftR DeleteShift(string id);
    ShiftR GetAllShifts(bool onlyActive = true);
    ShiftR RestoreShift(string id); // Implement RestoreElement method (async)
}