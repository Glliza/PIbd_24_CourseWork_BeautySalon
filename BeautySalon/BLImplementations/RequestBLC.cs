using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.Enums;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class RequestBLC : IRequestBLC
{
    private readonly IRequestSC _requestStorageContract;
    private readonly ILogger _logger;

    public RequestBLC(IRequestSC requestStorageContract, ILogger logger)
    {
        _requestStorageContract = requestStorageContract;
        _logger = logger;
    }

    public List<RequestDM> GetAllRequests(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllRequests params: {onlyActive}", onlyActive);
        return _requestStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<RequestDM> GetFilteredRequests(string? customerID = null, OrderStatus? status = null, DateTime? fromDateCreated = null, DateTime? toDateCreated = null)
    {
        _logger.LogInformation("GetFilteredRequests params: {customerID}, {status}, {fromDateCreated}, {toDateCreated}", customerID, status, fromDateCreated, toDateCreated);
        return _requestStorageContract.GetList(onlyActive: true, customerID, status, fromDateCreated, toDateCreated).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public RequestDM GetRequestById(string id)
    {
        _logger.LogInformation("GetRequestById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Request ID is not a valid GUID");
        }

        var result = _requestStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public RequestDM GetRequestWithItems(string id)
    {
        _logger.LogInformation("GetRequestWithItems for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Request ID is not a valid GUID");
        }

        var result = _requestStorageContract.GetRequestWithItemsAsync(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public void InsertRequest(RequestDM requestDataModel)
    {
        _logger.LogInformation("New request data: {json}", JsonSerializer.Serialize(requestDataModel));
        ArgumentNullException.ThrowIfNull(requestDataModel);
        requestDataModel.Validate();
        _requestStorageContract.AddElement(requestDataModel).GetAwaiter().GetResult();
    }

    public void UpdateRequest(RequestDM requestDataModel)
    {
        _logger.LogInformation("Update request data: {json}", JsonSerializer.Serialize(requestDataModel));
        ArgumentNullException.ThrowIfNull(requestDataModel);
        requestDataModel.Validate();
        _requestStorageContract.UpdElement(requestDataModel).GetAwaiter().GetResult();
    }

    public void DeleteRequest(string id)
    {
        _logger.LogInformation("Delete request by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Request ID is not a valid GUID");
        }
        _requestStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}