namespace BeautySalon.DataModels;

public class OrderBlock
{
    public int ID { get; set; }
    public int OrderID { get; set; } // FK - Order
    public int? ServiceID { get; set; }
    public int? ProductID { get; set; }

    // Количество и цена могут быть сведениями, полученными во время заказа/продажи (в DataAccess/DTOs)
    // decimal TotalSumm; - будет вычисленное значение или в информации о хранении
}
