namespace BeautySalon.DataModels;

public class Customer
{
    public int ID { get; set; }
    public string FIO { get; set; }
    public DateTime BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
}
