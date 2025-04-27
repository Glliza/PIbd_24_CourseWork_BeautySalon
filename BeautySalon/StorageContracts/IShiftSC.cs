using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IShiftSC
{
    // Get a list of shifts with optional filtering
    // Filters might include Staff, CashBox, Date/Time range, and active status
    Task<List<ShiftDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag OR DateTimeFinish == null
        string? shiftID = null, // Filter by specific Shift ID
        string? staffID = null,
        string? cashBoxID = null,
        DateTime? fromDateTimeStart = null,
        DateTime? toDateTimeStart = null,
        DateTime? fromDateTimeFinish = null,
        DateTime? toDateTimeFinish = null);

    Task<ShiftDM?> GetElementByID(string id);

    // Get the currently active shift for a specific staff member (DateTimeFinish is null)
    Task<ShiftDM?> GetActiveShiftForStaffAsync(string staffID);

    // Add a new shift (typically called when a staff member starts a shift)
    Task AddElement(ShiftDM shiftDataModel);

    // Update an existing shift (typically called when a staff member ends a shift to set DateTimeFinish)
    Task UpdElement(ShiftDM shiftDataModel);

    // Soft delete a shift by ID (e.g., archive an old shift record)
    Task DelElement(string id);

    Task RestoreElement(string id);

    // Optional: Method to explicitly end a shift (updates DateTimeFinish)
    Task EndShiftAsync(string shiftId, DateTime dateTimeFinish);
}
