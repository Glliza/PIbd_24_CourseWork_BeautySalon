using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ShiftDM(string id, string cashBoxId, string staffId, DateTime dateTimeStart, DateTime? dateTimeFinish) : IValidation
{
    public string ID { get; private set; } = id; // Primary Key
    public string CashBoxID { get; private set; } = cashBoxId; // FK to CashBoxDM
    public string StaffID { get; private set; } = staffId; // FK to StaffDM
    public DateTime DateTimeStart { get; private set; } = dateTimeStart;
    public DateTime? DateTimeFinish { get; private set; } = dateTimeFinish; // Nullable

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ShiftDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ShiftDM: ID is not a unique identifier");

        if (CashBoxID.IsEmpty()) throw new ValidationException("ShiftDM: CashBoxID is empty");
        if (!CashBoxID.IsGuid()) throw new ValidationException("ShiftDM: CashBoxID is not a unique identifier");

        if (StaffID.IsEmpty()) throw new ValidationException("ShiftDM: StaffID is empty");
        if (!StaffID.IsGuid()) throw new ValidationException("ShiftDM: StaffID is not a unique identifier");

        // Add validation for DateTimeStart/Finish consistency if needed
    }
}
