using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ServiceListItemDM(string requestIdOrVisitId, string serviceId, int quantityOrSessions, int totalItemDuration, decimal totalItemPrice) : IValidation // Renamed from ServiceList
{
    public string RequestID { get; private set; } = requestIdOrVisitId;

    public string ServiceID { get; private set; } = serviceId;

    // Amount/Quantity of this service (e.g., 5 sessions)
    public int QuantityOrSessions { get; private set; } = quantityOrSessions;

    public int TotalItemDuration { get; private set; } = totalItemDuration;
    // (e.g., Quantity * Service.DurationMinutes)

    public decimal TotalItemPrice { get; private set; } = totalItemPrice;
    // (e.g., Quantity * Service.BasePrice, potentially with discounts)

    public void Validate()
    {
        if (RequestID.IsEmpty()) throw new ValidationException("ServiceListItemDM: RequestOrVisitID is empty");
        if (!RequestID.IsGuid()) throw new ValidationException("ServiceListItemDM: RequestOrVisitID is not a unique identifier");
        if (ServiceID.IsEmpty()) throw new ValidationException("ServiceListItemDM: ServiceID is empty");
        if (!ServiceID.IsGuid()) throw new ValidationException("ServiceListItemDM: ServiceID is not a unique identifier");
        if (QuantityOrSessions <= 0) throw new ValidationException("ServiceListItemDM: QuantityOrSessions is less than or equal to 0");
        if (TotalItemDuration < 0) throw new ValidationException("ServiceListItemDM: TotalItemDuration cannot be negative"); // Can be 0 for services with no duration
        if (TotalItemPrice < 0) throw new ValidationException("ServiceListItemDM: TotalItemPrice cannot be negative"); // Assuming price cannot be negative
    }
}