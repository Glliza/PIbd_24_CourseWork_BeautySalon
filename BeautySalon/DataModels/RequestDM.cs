using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.Extensions;
using BeautySalon.Enums;

namespace BeautySalon.DataModels;

public class RequestDM(string id, string customerId, DateTime dateCreated, OrderStatus status, decimal totalPrice, List<ProductListItemDM> productItems, List<ServiceListItemDM> serviceItems) : IValidation
{
    public string ID { get; private set; } = id;

    public string CustomerID { get; private set; } = customerId; // can't be nullable

    public DateTime DateCreated { get; private set; } = dateCreated;
    // Should likely default to .UtcNow or similar if not provided [ ! ]

    public OrderStatus Status { get; private set; } = status;

    public decimal TotalPrice { get; private set; } = totalPrice; // Calculated [ * ]

    // List of product items included in this request
    public List<ProductListItemDM> ProductItems { get; private set; } = productItems;

    // List of services ...
    public List<ServiceListItemDM> ServiceItems { get; private set; } = serviceItems;


    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("RequestDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("RequestDM: ID is not a unique identifier");

        if (CustomerID.IsEmpty()) throw new ValidationException("RequestDM: CustomerID is empty");
        if (!CustomerID.IsGuid()) throw new ValidationException("RequestDM: CustomerID is not a unique identifier");

        if (TotalPrice < 0) throw new ValidationException("RequestDM: TotalPrice cannot be negative");
        // Add validation for ProductItems and ServiceItems lists [ * ]
    }
}