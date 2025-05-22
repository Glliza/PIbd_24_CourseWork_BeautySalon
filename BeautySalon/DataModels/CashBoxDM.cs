using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Extensions;

namespace BeautySalon.DataModels;

public class CashBoxDM(string id, decimal currentCapacity) : IValidation
{
    public string ID { get; private set; } = id;
    public decimal CurrentCapacity { get; private set; } = currentCapacity;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("CashBoxDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("CashBoxDM: ID is not a unique identifier");
        if (CurrentCapacity < 0) throw new ValidationException("CashBoxDM: CurrentCapacity cannot be negative");
    }
}