using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;
using BeautySalon.Enums;
using System.Text.RegularExpressions;

namespace BeautySalon.DataModels;

public class StaffDM(string id, string fio, PostType post, DateTime birthDate, string? password, string? email) : IValidation
{
    public string ID { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public PostType Post { get; private set; } = post;
    public DateTime BirthDate { get; private set; } = birthDate;
    public string? Password { get; private set; } = password;
    public string? Email { get; private set; } = email;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("StaffDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("StaffDM: ID is not a unique identifier");
        if (FIO.IsEmpty()) throw new ValidationException("StaffDM: FIO is empty");

        // Email Validation
        if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ValidationException("StaffDM: Invalid Email format");
        }

        // Password Validation
        if (string.IsNullOrEmpty(Password))
        {
            throw new ValidationException("StaffDM: Password cannot be empty");
        }
    }
}