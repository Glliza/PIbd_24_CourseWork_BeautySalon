using BeautySalon.Infrastructure;
using BeautySalon.Extensions;
using BeautySalon.Exceptions;

namespace BeautySalon.DataModels;

public class ServiceListHeader(string id) : IValidation
{
    public string ID { get; private set; } = id;

    // Note: The actual items (ServiceListItemDM) are linked back to this header via FK (RequestOrVisitID/?)
    // public List<ServiceListItemDM> Items { get; private set; } = new List<ServiceListItemDM>();

    public void Validate()
    {
        if (ID.IsEmpty()) throw new ValidationException("ServiceListHeaderDM: ID is empty");
        if (!ID.IsGuid()) throw new ValidationException("ServiceListHeaderDM: ID is not a unique identifier");
    }
}