using System.Text.RegularExpressions;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Extensions;

namespace BeautySalon.DataModels;

public class CustomerDM(string id, string fio, DateTime birthDate, string? phoneNumber, string? password, string? email) : IValidation
{
    public string ID { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public DateTime BirthDate { get; private set; } = birthDate;
    public string? PhoneNumber { get; private set; } = phoneNumber;
    public string? Password { get; private set; } = password;
    public string? Email { get; private set; } = email;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("CustomerDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("CustomerDM: ID is not a unique identifier");
        if (FIO.IsEmpty()) throw new ValidationException("CustomerDM: FIO is empty");
        if (PhoneNumber.IsEmpty()) throw new ValidationException("PhoneNumber is empty");
        if (!Regex.IsMatch(PhoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
            throw new ValidationException("CustomerDM: PhoneNumber is not a number");

        // Email Validation
        if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ValidationException("CustomerDM: Invalid Email format");
        }

        // Password Validation
        if (string.IsNullOrEmpty(Password))
        {
            throw new ValidationException("CustomerDM: Password cannot be empty");
        }
    }
}