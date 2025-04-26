namespace BeautySalon.DataModels;

public class Check
{
    public int ID { get; set; }
    public int? VisitID { get; set; } // FK - Visit, может быть NULL
    public int CashBoxID { get; set; } // FK - CashBox
    public int CustomerID { get; set; } // FK - Клиент, который платит (может отличаться от клиента Visit)
    public DateTime DateTimeIssued { get; set; } // когда выдан чек
    public decimal TotalSum { get; set; }
    // public PaymentMethod PaymentMethod { get; set; } - ( ??? )
}
