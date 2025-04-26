namespace BeautySalon.DataModels;
// Represents a line item on a Check (Receipt) - core concept
public class CheckItem
{
    public int ID { get; set; }
    public int CheckID { get; set; }
    public int? ServiceID { get; set; }
    public int? ProductID { get; set; }

    // same as for OrderBlock *
}