using BeautySalon.Infrastructure;
using BeautySalon.Extensions; // Correct namespace for extension methods
using BeautySalon.Exceptions; // Import ValidationException

namespace BeautySalon.DataModels;

// Represents a header for a list of products, referenced by other entities via FK
// Created because VisitDM requests a ProductList FK [ ! ]
public class ProductListHeader(string id) : IValidation
{
    public string ID { get; private set; } = id;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ProductListHeaderDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ProductListHeaderDM: ID is not a unique identifier");
    }
}
