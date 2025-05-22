using System.Text.RegularExpressions;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Extensions;

namespace BeautySalon.DataModels;

public class CustomerDM(string id, string fio, DateTime birthDate, string? phoneNumber) : IValidation
{
    public string ID { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public DateTime BirthDate { get; private set; } = birthDate;
    public string? PhoneNumber { get; private set; } = phoneNumber;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("CustomerDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("CustomerDM: ID is not a unique identifier");
        if (FIO.IsEmpty()) throw new ValidationException("CustomerDM: FIO is empty");
        if (PhoneNumber.IsEmpty()) throw new ValidationException("PhoneNumber is empty");
        if (!Regex.IsMatch(PhoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
            throw new ValidationException("CustomerDM: PhoneNumber is not a number");
    }
}