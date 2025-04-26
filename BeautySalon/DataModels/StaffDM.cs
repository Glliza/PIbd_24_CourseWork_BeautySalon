using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Enums;

namespace BeautySalon.DataModels;

public class StaffDM(string id, string fio, PostType post, DateTime birthDate) : IValidation
{
    public string ID { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public PostType Post { get; private set; } = post;
    public DateTime BirthDate { get; private set; } = birthDate;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("StaffDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("StaffDM: ID is not a unique identifier");
        if (FIO.IsEmpty()) throw new ValidationException("StaffDM: FIO is empty");
    }
}

