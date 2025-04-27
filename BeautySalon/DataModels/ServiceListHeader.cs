using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

// Represents a header for a list of services, referenced by other entities via FK
// Created because RequestDM and VisitDM request a ServiceList FK
public class ServiceListHeader(string id) : IValidation
{
    public string ID { get; private set; } = id; // Primary Key

    // Note: The actual items (ServiceListItemDM) are linked back to this header via FK (RequestOrVisitID/?)
    // public List<ServiceListItemDM> Items { get; private set; } = new List<ServiceListItemDM>();

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ServiceListHeaderDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ServiceListHeaderDM: ID is not a unique identifier");
    }
}
