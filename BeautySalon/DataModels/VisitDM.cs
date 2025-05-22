using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

// NOTE: This structure is based on the fields you provided, linking via FKs to List Headers.
// This differs from ReceiptDM/RequestDM holding Lists directly.
public class VisitDM(string id, string customerId, string staffId, string? productListId, string serviceListId, bool status, DateTime dateTimeOfVisit, decimal totalPrice) : IValidation // Corrected field names and types
{
    public string ID { get; private set; } = id;

    public string CustomerID { get; private set; } = customerId;

    public string StaffID { get; private set; } = staffId;

    // FK to a ServiceListHeaderDM - Required as per requirement [ ! ]
    public string ServiceListID { get; private set; } = serviceListId;

    public bool Status { get; private set; } = status;
    // OrderStatus goes only for Request (full check of services and items) [ ! ]

    public DateTime DateTimeOfVisit { get; private set; } = dateTimeOfVisit;

    public decimal TotalPrice { get; private set; } = totalPrice;


    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("VisitDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("VisitDM: ID is not a unique identifier");

        if (CustomerID.IsEmpty()) throw new ValidationException("VisitDM: CustomerID is empty");
        if (!CustomerID.IsGuid()) throw new ValidationException("VisitDM: CustomerID is not a unique identifier");

        if (StaffID.IsEmpty()) throw new ValidationException("VisitDM: StaffID is empty");
        if (!StaffID.IsGuid()) throw new ValidationException("VisitDM: StaffID is not a unique identifier");

        if (ServiceListID.IsEmpty()) throw new ValidationException("VisitDM: ServiceListID is empty");
        if (!ServiceListID.IsGuid()) throw new ValidationException("VisitDM: ServiceListID is not a unique identifier");

        if (TotalPrice < 0) throw new ValidationException("VisitDM: TotalPrice cannot be negative");
        // Add validation for DateTimeOfVisit [ * ]
    }
}