
using BeautySalon.Enums;

namespace BeautySalon.DataModels;

public class Staff(string id, string fio, PostType postType, DateTime birthDate)
{
    public string Id { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public PostType PostType { get; private set; } = postType;
    public DateTime BirthDate { get; private set; } = birthDate;

}
