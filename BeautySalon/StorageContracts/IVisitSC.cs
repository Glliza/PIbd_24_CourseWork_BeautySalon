using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IVisitSC
{
    Task<List<VisitDM>> GetList(
        bool onlyActive = true, 
        string? customerID = null,
        string? staffID = null,
        bool? status = null,
        DateTime? fromDateTimeOfVisit = null,
        DateTime? toDateTimeOfVisit = null);

    Task<VisitDM?> GetElementByID(string id);
    Task AddElement(VisitDM visitDataModel);
    Task UpdElement(VisitDM visitDataModel);
    Task DelElement(string id);
}
