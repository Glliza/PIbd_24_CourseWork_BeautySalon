using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IVisitAdapter
{
    VisitR GetVisitById(string id);
    VisitR CreateVisit(VisitVM visitModel);
    VisitR UpdateVisit(VisitVM visitModel);
    VisitR CancelVisit(string id);
    VisitR GetAllVisits(bool onlyActive = true);
}