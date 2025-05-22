using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.BusinessLogicContracts;

public interface IRequestBLC
{
    List<RequestDM> GetAllRequests(bool onlyActive = true);
    List<RequestDM> GetFilteredRequests(
        string? customerID = null,
        OrderStatus? status = null,
        DateTime? fromDateCreated = null,
        DateTime? toDateCreated = null);
    RequestDM GetRequestById(string id);
    RequestDM GetRequestWithItems(string id);
    void InsertRequest(RequestDM requestDataModel);
    void UpdateRequest(RequestDM requestDataModel);
    void DeleteRequest(string id);
}