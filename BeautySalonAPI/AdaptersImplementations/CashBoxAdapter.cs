using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class CashBoxAdapter : ICashBoxAdapter
{
    private readonly ICashBoxBLC _cashBoxBusinessLogic;
    private readonly ILogger<CashBoxAdapter> _logger;
    private readonly IMapper _mapper;

    public CashBoxAdapter(ICashBoxBLC cashBoxBusinessLogic, ILogger<CashBoxAdapter> logger, IMapper mapper)
    {
        _cashBoxBusinessLogic = cashBoxBusinessLogic ?? throw new ArgumentNullException(nameof(cashBoxBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    private CashBoxR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<CashBoxR>(message);
    }

    public CashBoxR GetCashBoxById(string id)
    {
        try
        {
            var cashBoxDataModel = _cashBoxBusinessLogic.GetCashBoxById(id);
            if (cashBoxDataModel == null)
            {
                return HandleNotFound($"CashBox not found with id: {id}");
            }
            var cashBoxViewModel = _mapper.Map<CashBoxVM>(cashBoxDataModel);
            return OperationResponseBase.OK<CashBoxR, CashBoxVM>(cashBoxViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetCashBoxById");
            return OperationResponseBase.BadRequest<CashBoxR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetCashBoxById");
            return HandleNotFound($"CashBox not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetCashBoxById");
            return OperationResponseBase.InternalServerError<CashBoxR>(ex.Message);
        }
    }

    public CashBoxR CreateCashBox(CashBoxVM cashBoxModel)
    {
        try
        {
            var cashBoxDataModel = _mapper.Map<CashBoxDM>(cashBoxModel);
            if (cashBoxDataModel == null)
            {
                _logger.LogError("Mapping failed from CashBoxViewModel to CashBoxDataModel in CreateCashBox");
                return OperationResponseBase.BadRequest<CashBoxR>("Invalid cashBox data provided.");
            }
            _cashBoxBusinessLogic.InsertCashBox(cashBoxDataModel);
            var createdCashBox = _cashBoxBusinessLogic.GetCashBoxById(cashBoxDataModel.ID);
            if (createdCashBox == null)
            {
                return OperationResponseBase.NotFound<CashBoxR>($"CashBox not found with id: {cashBoxModel.Id}");
            }
            var viewModel = _mapper.Map<CashBoxVM>(createdCashBox);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from CashBoxDataModel to CashBoxViewModel in CreateCashBox");
                return OperationResponseBase.InternalServerError<CashBoxR>("Failed to map created cashBox data.");
            }
            return OperationResponseBase.OK<CashBoxR, CashBoxVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateCashBox");
            return OperationResponseBase.BadRequest<CashBoxR>("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateCashBox");
            return OperationResponseBase.BadRequest<CashBoxR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateCashBox");
            return OperationResponseBase.InternalServerError<CashBoxR>(ex.Message);
        }
    }

    public CashBoxR UpdateCashBox(CashBoxVM cashBoxModel)
    {
        try
        {
            var cashBoxDataModel = _mapper.Map<CashBoxDM>(cashBoxModel);
            _cashBoxBusinessLogic.UpdateCashBox(cashBoxDataModel);
            return OperationResponseBase.OK<CashBoxR, CashBoxVM>(cashBoxModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateCashBox");
            return OperationResponseBase.BadRequest<CashBoxR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateCashBox");
            return OperationResponseBase.NotFound<CashBoxR>($"CashBox not found with id: {cashBoxModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateCashBox");
            return OperationResponseBase.InternalServerError<CashBoxR>(ex.Message);
        }
    }

    public CashBoxR DeleteCashBox(string id)
    {
        try
        {
            _cashBoxBusinessLogic.DeleteCashBox(id);
            return OperationResponseBase.NoContent<CashBoxR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteCashBox");
            return OperationResponseBase.NotFound<CashBoxR>($"CashBox not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteCashBox");
            return OperationResponseBase.InternalServerError<CashBoxR>(ex.Message);
        }
    }

    public CashBoxR GetAllCashBoxes(bool onlyActive = true)
    {
        try
        {
            var cashBoxDataModels = _cashBoxBusinessLogic.GetAllCashBoxes(onlyActive);
            var cashBoxViewModels = _mapper.Map<List<CashBoxVM>>(cashBoxDataModels);
            return OperationResponseBase.OK<CashBoxR, List<CashBoxVM>>(cashBoxViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllCashBoxes");
            return OperationResponseBase.InternalServerError<CashBoxR>(ex.Message);
        }
    }
}