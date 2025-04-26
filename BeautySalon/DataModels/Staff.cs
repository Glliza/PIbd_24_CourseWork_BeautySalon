using BeautySalon.Enums;
namespace BeautySalon.DataModels;

public class Staff
{
    public string Id { get; private set; }
    public string FIO { get; private set; }
    public PostType PostType { get; private set; }
    public DateTime BirthDate { get; private set; }
}
