using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Enums;

namespace BeautySalon.DataModels;

public class ProductDM(string id, string name, ProductType type, string? description, int stockQuantity, decimal pricePerOne) : IValidation
{
    public string ID { get; private set; } = id;
    public string Name { get; private set; } = name;
    public ProductType ProductType { get; private set; } = type;
    public string? Description { get; private set; } = description;
    public int StockQuantity { get; private set; } = stockQuantity;
    public decimal PricePerOne { get; private set; } = pricePerOne;

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ProductDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ProductDM: ID is not a unique identifier");
        if (Name.IsEmpty()) throw new ValidationException("ProductDM: Name is empty");
        if (StockQuantity < 0) throw new ValidationException("ProductDM: StockQuantity cannot be negative");
        if (PricePerOne < 0) throw new ValidationException("ProductDM: PricePerOne cannot be negative");
    }
}
