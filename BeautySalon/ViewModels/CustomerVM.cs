namespace BeautySalon.ViewModels;

public class CustomerVM
{
    public required string Id { get; set; }
    public required string FIO { get; set; }
    public required string Birthdate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}