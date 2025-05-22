using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ProductListItemDM(string receiptOrRequestId, string productId, int amount) : IValidation
{
    // Using 'OrRequestId' to note it can belong to either [ ! ]
    public string ReceiptOrRequestID { get; private set; } = receiptOrRequestId;

    public string ProductID { get; private set; } = productId;

    public int Amount { get; private set; } = amount;

    public void Validate()
    {
        if (ReceiptOrRequestID.IsEmpty()) throw new ValidationException("ProductListItemDM: ReceiptOrRequestID is empty");
        if (!ReceiptOrRequestID.IsGuid()) throw new ValidationException("ProductListItemDM: ReceiptOrRequestID is not a unique identifier");
        if (ProductID.IsEmpty()) throw new ValidationException("ProductListItemDM: ProductID is empty");
        if (!ProductID.IsGuid()) throw new ValidationException("ProductListItemDM: ProductID is not a unique identifier");
        if (Amount <= 0) throw new ValidationException("ProductListItemDM: Amount is less than or equal to 0");
    }
}