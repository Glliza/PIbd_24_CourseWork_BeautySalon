using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class VisitAdapter : IVisitAdapter
{
    private readonly IVisitBLC _visitBusinessLogic;
    private readonly ILogger<VisitAdapter> _logger;
    private readonly IMapper _mapper;

    public VisitAdapter(IVisitBLC visitBusinessLogic, ILogger<VisitAdapter> logger, IMapper mapper)
    {
        _visitBusinessLogic = visitBusinessLogic ?? throw new ArgumentNullException(nameof(visitBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    private VisitR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<VisitR>(message);
    }

    public VisitR GetVisitById(string id)
    {
        try
        {
            var visitDataModel = _visitBusinessLogic.GetVisitById(id);
            if (visitDataModel == null)
            {
                return HandleNotFound($"Visit not found with id: {id}");
            }
            var visitViewModel = _mapper.Map<VisitVM>(visitDataModel);
            return OperationResponseBase.OK<VisitR, VisitVM>(visitViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetVisitById");
            return OperationResponseBase.BadRequest<VisitR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetVisitById");
            return HandleNotFound($"Visit not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetVisitById");
            return OperationResponseBase.InternalServerError<VisitR>(ex.Message);
        }
    }

    public VisitR CreateVisit(VisitVM visitModel)
    {
        try
        {
            var visitDataModel = _mapper.Map<VisitDM>(visitModel);
            if (visitDataModel == null)
            {
                _logger.LogError("Mapping failed from VisitViewModel to VisitDataModel in CreateVisit");
                return OperationResponseBase.BadRequest<VisitR>("Invalid visit data provided.");
            }
            _visitBusinessLogic.InsertVisit(visitDataModel);
            var createdVisit = _visitBusinessLogic.GetVisitById(visitDataModel.ID);
            if (createdVisit == null)
            {
                return OperationResponseBase.NotFound<VisitR>($"Visit not found with id: {visitModel.Id}");
            }
            var viewModel = _mapper.Map<VisitVM>(createdVisit);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from VisitDataModel to VisitViewModel in CreateVisit");
                return OperationResponseBase.InternalServerError<VisitR>("Failed to map created visit data.");
            }
            return OperationResponseBase.OK<VisitR, VisitVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateVisit");
            return OperationResponseBase.BadRequest<VisitR>("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateVisit");
            return OperationResponseBase.BadRequest<VisitR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateVisit");
            return OperationResponseBase.InternalServerError<VisitR>(ex.Message);
        }
    }

    public VisitR UpdateVisit(VisitVM visitModel)
    {
        try
        {
            var visitDataModel = _mapper.Map<VisitDM>(visitModel);
            _visitBusinessLogic.UpdateVisit(visitDataModel);
            return OperationResponseBase.OK<VisitR, VisitVM>(visitModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateVisit");
            return OperationResponseBase.BadRequest<VisitR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateVisit");
            return OperationResponseBase.NotFound<VisitR>($"Visit not found with id: {visitModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateVisit");
            return OperationResponseBase.InternalServerError<VisitR>(ex.Message);
        }
    }

    public VisitR CancelVisit(string id)
    {
        try
        {
            _visitBusinessLogic.DeleteVisit(id);
            return OperationResponseBase.NoContent<VisitR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in CancelVisit");
            return OperationResponseBase.NotFound<VisitR>($"Visit not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CancelVisit");
            return OperationResponseBase.InternalServerError<VisitR>(ex.Message);
        }
    }

    public VisitR GetAllVisits(bool onlyActive = true)
    {
        try
        {
            var visitDataModels = _visitBusinessLogic.GetAllVisits(onlyActive);
            var visitViewModels = _mapper.Map<List<VisitVM>>(visitDataModels);
            return OperationResponseBase.OK<VisitR, List<VisitVM>>(visitViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllVisits");
            return OperationResponseBase.InternalServerError<VisitR>(ex.Message);
        }
    }
}