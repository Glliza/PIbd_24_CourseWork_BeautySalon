using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Enums;

namespace BeautySalon.DataModels;

public class RequestDM(string id, string customerId, DateTime dateCreated, OrderStatus status, decimal totalPrice, List<ProductListItemDM> productItems, List<ServiceListItemDM> serviceItems) : IValidation
{
    public string ID { get; private set; } = id; // Primary Key

    public string CustomerID { get; private set; } = customerId; // FK to CustomerDM

    public DateTime DateCreated { get; private set; } = dateCreated; // Should likely default to UtcNow or similar if not provided

    public OrderStatus Status { get; private set; } = status; // Using OrderStatus enum

    public decimal TotalPrice { get; private set; } = totalPrice; // Use decimal. Calculated.

    // List of product items included in this request
    public List<ProductListItemDM> ProductItems { get; private set; } = productItems;

    // List of service items included in this request
    public List<ServiceListItemDM> ServiceItems { get; private set; } = serviceItems;


    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("RequestDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("RequestDM: ID is not a unique identifier");

        if (CustomerID.IsEmpty()) throw new ValidationException("RequestDM: CustomerID is empty");
        if (!CustomerID.IsGuid()) throw new ValidationException("RequestDM: CustomerID is not a unique identifier");

        if (TotalPrice < 0) throw new ValidationException("RequestDM: TotalPrice cannot be negative"); // Can be 0 if items are free?
        // Add validation for ProductItems and ServiceItems lists if needed
    }
}
