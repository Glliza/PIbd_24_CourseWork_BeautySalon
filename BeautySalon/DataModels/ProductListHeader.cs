using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

// Represents a header for a list of products, referenced by other entities via FK
// Created because VisitDM requests a ProductList FK
public class ProductListHeader(string id) : IValidation
{
    public string ID { get; private set; } = id; // Primary Key

    // Note: The actual items (ProductListItemDM) are linked back to this header via FK (ReceiptOrRequestID/?)
    // A list property here is for conceptual clarity in the model layer, but requires careful mapping in DataAccess
    // public List<ProductListItemDM> Items { get; private set; } = new List<ProductListItemDM>();

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ProductListHeaderDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ProductListHeaderDM: ID is not a unique identifier");
    }
}
