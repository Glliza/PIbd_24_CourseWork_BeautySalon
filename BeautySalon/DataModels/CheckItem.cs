namespace BeautySalon.DataModels;
public class CheckItem
{
    public int ID { get; set; }
    public int CheckID { get; set; }

    // смысл тот же, что и в OrderBlock 
    public int? ServiceID { get; set; }
    public int? ProductID { get; set; }

    public int Quantity { get; set; } // !
    public decimal TotalPrice { get; set; }
}
