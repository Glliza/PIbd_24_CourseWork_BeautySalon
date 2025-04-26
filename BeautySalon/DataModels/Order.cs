using BeautySalon.Enums;
namespace BeautySalon.DataModels;

public class Order
{
    public int ID { get; set; } // Primary Key
    public int CustomerID { get; set; } // FK к сущности Customer
    public DateTime DateCreated { get; set; }
    public OrderStatus Status { get; set; } // см. прошлое сообщение
    public decimal TotalPrice { get; set; }
}
