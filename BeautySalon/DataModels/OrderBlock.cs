namespace BeautySalon.DataModels;

public class OrderBlock
{
    public int ID { get; set; }
    public int OrderID { get; set; } // FK к сущ.-ти Order

    // Взаимозаменяемые: (либо услуга, либо список товаров пусты)
    public int? ServiceID { get; set; } // FK - Service
    public int? ProductID { get; set; } // FK - Product

    public decimal TotalSumm { get; set; }
}
