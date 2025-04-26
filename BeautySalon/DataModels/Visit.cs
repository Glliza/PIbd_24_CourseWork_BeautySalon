namespace BeautySalon.DataModels;
public class Visit
{
    public int ID { get; set; } // Primary Key
    public int? OrderID { get; set; } // FK to Order, Nullable: for ad-hoc visits not linked to a prior order
    public int CustomerID { get; set; } // FK to Customer
    public int StaffID { get; set; } // FK to Staff performing the service(s)
    public DateTime DateTimeOfVisit { get; set; }
    // ( ??? ) - public int Status { get; set; }
    // Цена будет определяться на основе соответствующего чека(-ов).
}
