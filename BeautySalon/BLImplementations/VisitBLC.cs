using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class VisitBLC : IVisitBLC
{
    private readonly IVisitSC _visitStorageContract;
    private readonly ILogger _logger;

    public VisitBLC(IVisitSC visitStorageContract, ILogger logger)
    {
        _visitStorageContract = visitStorageContract;
        _logger = logger;
    }

    public List<VisitDM> GetAllVisits(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllVisits params: {onlyActive}", onlyActive);
        return _visitStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public List<VisitDM> GetFilteredVisits(string? customerID = null, string? staffID = null, bool? status = null, DateTime? fromDateTimeOfVisit = null, DateTime? toDateTimeOfVisit = null)
    {
        _logger.LogInformation("GetFilteredVisits params: {customerID}, {staffID}, {status}, {fromDateTimeOfVisit}, {toDateTimeOfVisit}", customerID, staffID, status, fromDateTimeOfVisit, toDateTimeOfVisit);
        return _visitStorageContract.GetList(onlyActive: true, customerID, staffID, status, fromDateTimeOfVisit, toDateTimeOfVisit).GetAwaiter().GetResult()
               ?? throw new NullListException();
    }

    public VisitDM GetVisitById(string id)
    {
        _logger.LogInformation("GetVisitById for {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Visit ID is not a valid GUID");
        }

        var result = _visitStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public void InsertVisit(VisitDM visitDataModel)
    {
        _logger.LogInformation("New visit data: {json}", JsonSerializer.Serialize(visitDataModel));
        ArgumentNullException.ThrowIfNull(visitDataModel);
        visitDataModel.Validate();
        _visitStorageContract.AddElement(visitDataModel).GetAwaiter().GetResult();
    }

    public void UpdateVisit(VisitDM visitDataModel)
    {
        _logger.LogInformation("Update visit data: {json}", JsonSerializer.Serialize(visitDataModel));
        ArgumentNullException.ThrowIfNull(visitDataModel);
        visitDataModel.Validate();
        _visitStorageContract.UpdElement(visitDataModel).GetAwaiter().GetResult();
    }

    public void DeleteVisit(string id)
    {
        _logger.LogInformation("Delete visit by id: {id}", id);
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Visit ID is not a valid GUID");
        }
        _visitStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}