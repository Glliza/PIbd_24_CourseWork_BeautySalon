using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ReceiptDM(string id, string cashboxId, string staffId, string? customerId, decimal totalSumm, bool isCanceled, List<ProductListItemDM> products, List<ServiceListItemDM> services) : IValidation
{
    public string ID { get; private set; } = id;

    public string CashBoxID { get; private set; } = cashboxId;

    public string StaffID { get; private set; } = staffId; // FK to StaffDM

    public string? CustomerID { get; private set; } = customerId; // FK to CustomerDM (Nullable)

    public DateTime DateIssued { get; private set; } = DateTime.UtcNow;

    public decimal TotalSumm { get; private set; } = totalSumm;

    public bool IsCanceled { get; private set; } = isCanceled;

    // List of product items included in this receipt
    // Note: This model implies Receipt only contains Products, not Services, based on the list type.
    public List<ProductListItemDM> Products { get; private set; } = products;

    // public List<ServiceListItemDM> Services { get; private set; } = services; [ ? ]
    // + Missing: optional link to VisitDM

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ReceiptDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ReceiptDM: The value in the field ID is not a unique identifier");

        if (StaffID.IsEmpty()) throw new ValidationException("ReceiptDM: StaffID is empty");
        if (!StaffID.IsGuid()) throw new ValidationException("ReceiptDM: The value in the field StaffID is not a unique identifier");

        // Check CustomerID only if it's not empty (if provided, must be a GUID)
        if (!CustomerID.IsEmpty() && !CustomerID.IsGuid())
            throw new ValidationException("ReceiptDM: The value in the field CustomerID is not a unique identifier");

        if (TotalSumm <= 0) throw new ValidationException("ReceiptDM: Total Summ is less than or equal to 0");
        if ((Products?.Count ?? 0) == 0) throw new ValidationException("ReceiptDM: The receipt must include products");
    }
}
