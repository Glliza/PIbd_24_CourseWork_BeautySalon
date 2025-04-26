using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ServiceDM(string id, string name, string? description, decimal basePrice, int durationMinutes) : IValidation
{
    public string ID { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public decimal BasePrice { get; private set; } = basePrice;
    public int DurationMinutes { get; private set; } = durationMinutes;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ServiceDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ServiceDM: ID is not a unique identifier");
        if (Name.IsEmpty()) throw new ValidationException("ServiceDM: Name is empty");
        if (BasePrice < 0) throw new ValidationException("ServiceDM: BasePrice cannot be negative"); // Assuming price cannot be negative
        if (DurationMinutes <= 0) throw new ValidationException("ServiceDM: DurationMinutes must be positive");
    }
}
