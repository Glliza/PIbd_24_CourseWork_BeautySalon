using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ServiceListItemDM(string requestIdOrVisitId, string serviceId, int quantityOrSessions, int totalItemDuration, decimal totalItemPrice) : IValidation // Renamed from ServiceList
{
    // FK to the parent entity (RequestDM or VisitDM)
    public string RequestOrVisitID { get; private set; } = requestIdOrVisitId; // FK

    public string ServiceID { get; private set; } = serviceId; // FK to ServiceDM

    // Amount/Quantity of this service (e.g., 1 session, 3 sessions)
    public int QuantityOrSessions { get; private set; } = quantityOrSessions;

    // Calculated total duration for this line item (e.g., Quantity * Service.DurationMinutes)
    public int TotalItemDuration { get; private set; } = totalItemDuration;

    // Calculated total price for this line item (e.g., Quantity * Service.BasePrice, potentially with discounts)
    public decimal TotalItemPrice { get; private set; } = totalItemPrice; // Use decimal

    public void Validate()
    {
        if (RequestOrVisitID.IsEmpty()) throw new ValidationException("ServiceListItemDM: RequestOrVisitID is empty");
        if (!RequestOrVisitID.IsGuid()) throw new ValidationException("ServiceListItemDM: RequestOrVisitID is not a unique identifier");
        if (ServiceID.IsEmpty()) throw new ValidationException("ServiceListItemDM: ServiceID is empty");
        if (!ServiceID.IsGuid()) throw new ValidationException("ServiceListItemDM: ServiceID is not a unique identifier");
        if (QuantityOrSessions <= 0) throw new ValidationException("ServiceListItemDM: QuantityOrSessions is less than or equal to 0");
        if (TotalItemDuration < 0) throw new ValidationException("ServiceListItemDM: TotalItemDuration cannot be negative"); // Can be 0 for services with no duration
        if (TotalItemPrice < 0) throw new ValidationException("ServiceListItemDM: TotalItemPrice cannot be negative"); // Assuming price cannot be negative
    }
}
