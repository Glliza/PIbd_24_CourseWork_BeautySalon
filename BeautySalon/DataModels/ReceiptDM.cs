using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ReceiptDM(string id, string cashboxId, string staffId, string? customerId, decimal totalSumm, bool isCanceled, List<ProductListItemDM> products, List<ServiceListItemDM> services) : IValidation
{
    public string ID { get; private set; } = id;

    public string CashBoxID { get; private set; } = cashboxId;

    public string StaffID { get; private set; } = staffId;

    public string? CustomerID { get; private set; } = customerId; // can be nullable

    public DateTime DateIssued { get; private set; } = DateTime.UtcNow;

    public decimal TotalSumm { get; private set; } = totalSumm;

    public bool IsCanceled { get; private set; } = isCanceled;

    public List<ProductListItemDM> Products { get; private set; } = products;

    // public List<ServiceListItemDM> Services { get; private set; } = services; [ ? ] - goes for request

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ReceiptDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ReceiptDM: The value in the field ID is not a unique identifier");

        if (StaffID.IsEmpty()) throw new ValidationException("ReceiptDM: StaffID is empty");
        if (!StaffID.IsGuid()) throw new ValidationException("ReceiptDM: The value in the field StaffID is not a unique identifier");

        if (!CustomerID.IsEmpty() && !CustomerID.IsGuid())
            throw new ValidationException("ReceiptDM: The value in the field CustomerID is not a unique identifier");

        if (TotalSumm <= 0) throw new ValidationException("ReceiptDM: Total Summ is less than or equal to 0");
        if ((Products?.Count ?? 0) == 0) throw new ValidationException("ReceiptDM: The receipt must include products");
    }
}