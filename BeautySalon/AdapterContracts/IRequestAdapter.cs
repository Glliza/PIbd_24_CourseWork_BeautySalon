using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IRequestAdapter
{
    RequestR GetRequestById(string id);
    RequestR CreateRequest(RequestVM requestModel);
    RequestR UpdateRequest(RequestVM requestModel);
    RequestR DeleteRequest(string id);
    RequestR GetAllRequests(bool onlyActive = true);
}