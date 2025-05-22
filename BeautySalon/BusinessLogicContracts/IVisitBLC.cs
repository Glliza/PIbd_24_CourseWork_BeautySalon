using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogicContracts;

public interface IVisitBLC
{
    List<VisitDM> GetAllVisits(bool onlyActive = true);
    List<VisitDM> GetFilteredVisits(
        string? customerID = null,
        string? staffID = null,
        bool? status = null,
        DateTime? fromDateTimeOfVisit = null,
        DateTime? toDateTimeOfVisit = null);
    VisitDM GetVisitById(string id);
    void InsertVisit(VisitDM visitDataModel);
    void UpdateVisit(VisitDM visitDataModel);
    void DeleteVisit(string id);
}