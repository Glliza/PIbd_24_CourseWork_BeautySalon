using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ProductListItemDM(string receiptOrRequestId, string productId, int amount) : IValidation // Renamed from ProductListDM
{
    // FK to the parent entity (ReceiptDM or RequestDM) - Name indicates relationship
    // Using 'OrRequestId' to note it can belong to either
    public string ReceiptOrRequestID { get; private set; } = receiptOrRequestId; // FK

    public string ProductID { get; private set; } = productId; // FK to ProductDM

    public int Amount { get; private set; } = amount; // Quantity

    public void Validate()
    {
        if (ReceiptOrRequestID.IsEmpty()) throw new ValidationException("ProductListItemDM: ReceiptOrRequestID is empty");
        if (!ReceiptOrRequestID.IsGuid()) throw new ValidationException("ProductListItemDM: ReceiptOrRequestID is not a unique identifier");
        if (ProductID.IsEmpty()) throw new ValidationException("ProductListItemDM: ProductID is empty");
        if (!ProductID.IsGuid()) throw new ValidationException("ProductListItemDM: ProductID is not a unique identifier");
        if (Amount <= 0) throw new ValidationException("ProductListItemDM: Amount is less than or equal to 0");
    }
}