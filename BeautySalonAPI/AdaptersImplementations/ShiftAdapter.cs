using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class ShiftAdapter : IShiftAdapter
{
    private readonly IShiftBLC _shiftBusinessLogic;
    private readonly ILogger<ShiftAdapter> _logger;
    private readonly IMapper _mapper;

    public ShiftAdapter(IShiftBLC shiftBusinessLogic, ILogger<ShiftAdapter> logger, IMapper mapper)
    {
        _shiftBusinessLogic = shiftBusinessLogic ?? throw new ArgumentNullException(nameof(shiftBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    private ShiftR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<ShiftR>(message);
    }

    public ShiftR GetShiftById(string id)
    {
        try
        {
            var shiftDataModel = _shiftBusinessLogic.GetShiftById(id);
            if (shiftDataModel == null)
            {
                return HandleNotFound($"Shift not found with id: {id}");
            }
            var shiftViewModel = _mapper.Map<ShiftVM>(shiftDataModel);
            return OperationResponseBase.OK<ShiftR, ShiftVM>(shiftViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetShiftById");
            return OperationResponseBase.BadRequest<ShiftR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetShiftById");
            return HandleNotFound($"Shift not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetShiftById");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR GetActiveShiftForStaff(string staffID)
    {
        try
        {
            var shiftDataModel = _shiftBusinessLogic.GetActiveShiftForStaff(staffID);
            if (shiftDataModel == null)
            {
                return HandleNotFound($"No active shift found for staff with id: {staffID}");
            }
            var shiftViewModel = _mapper.Map<ShiftVM>(shiftDataModel);
            return OperationResponseBase.OK<ShiftR, ShiftVM>(shiftViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetActiveShiftForStaff");
            return OperationResponseBase.BadRequest<ShiftR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetActiveShiftForStaff");
            return HandleNotFound($"No active shift found for staff with id: {staffID}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetActiveShiftForStaff");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR StartShift(ShiftVM shiftModel)
    {
        try
        {
            var shiftDataModel = _mapper.Map<ShiftDM>(shiftModel);
            if (shiftDataModel == null)
            {
                _logger.LogError("Mapping failed from ShiftViewModel to ShiftDataModel in StartShift");
                return OperationResponseBase.BadRequest<ShiftR>("Invalid shift data provided.");
            }
            _shiftBusinessLogic.InsertShift(shiftDataModel);
            var createdShift = _shiftBusinessLogic.GetShiftById(shiftDataModel.ID);
            if (createdShift == null)
            {
                return OperationResponseBase.NotFound<ShiftR>($"Shift not found with id: {shiftModel.Id}");
            }
            var viewModel = _mapper.Map<ShiftVM>(createdShift);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ShiftDataModel to ShiftViewModel in StartShift");
                return OperationResponseBase.InternalServerError<ShiftR>("Failed to map created shift data.");
            }
            return OperationResponseBase.OK<ShiftR, ShiftVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in StartShift");
            return OperationResponseBase.BadRequest<ShiftR>("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in StartShift");
            return OperationResponseBase.BadRequest<ShiftR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in StartShift");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR UpdateShift(ShiftVM shiftModel)
    {
        try
        {
            var shiftDataModel = _mapper.Map<ShiftDM>(shiftModel);
            _shiftBusinessLogic.UpdateShift(shiftDataModel);
            return OperationResponseBase.OK<ShiftR, ShiftVM>(shiftModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateShift");
            return OperationResponseBase.BadRequest<ShiftR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateShift");
            return OperationResponseBase.NotFound<ShiftR>($"Shift not found with id: {shiftModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateShift");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR DeleteShift(string id)
    {
        try
        {
            _shiftBusinessLogic.DeleteShift(id);
            return OperationResponseBase.NoContent<ShiftR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteShift");
            return OperationResponseBase.NotFound<ShiftR>($"Shift not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteShift");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR RestoreShift(string id)
    {
        try
        {
            _shiftBusinessLogic.RestoreShift(id);
            // Assuming we need to return some data related to the shift
            var restoredShift = _shiftBusinessLogic.GetShiftById(id);
            if (restoredShift == null)
            {
                _logger.LogError("Failed to restore shift with id: {ShiftId}", id);
                return OperationResponseBase.InternalServerError<ShiftR>("Failed to restore shift.");
            }
            var viewModel = _mapper.Map<ShiftVM>(restoredShift);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ShiftDataModel to ShiftViewModel in RestoreShift");
                return OperationResponseBase.InternalServerError<ShiftR>("Failed to map restored shift data.");
            }
            return OperationResponseBase.OK<ShiftR, ShiftVM>(viewModel);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in RestoreShift");
            return OperationResponseBase.NotFound<ShiftR>($"Shift not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in RestoreShift");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR EndShift(string shiftId, DateTime dateTimeFinish)
    {
        try
        {
            _shiftBusinessLogic.EndShift(shiftId, dateTimeFinish);
            // Assuming we need to return some data related to the shift
            var endedShift = _shiftBusinessLogic.GetShiftById(shiftId);
            if (endedShift == null)
            {
                _logger.LogError("Failed to end shift with id: {ShiftId}", shiftId);
                return OperationResponseBase.InternalServerError<ShiftR>("Failed to end shift.");
            }
            var viewModel = _mapper.Map<ShiftVM>(endedShift);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ShiftDataModel to ShiftViewModel in EndShift");
                return OperationResponseBase.InternalServerError<ShiftR>("Failed to map ended shift data.");
            }
            return OperationResponseBase.OK<ShiftR, ShiftVM>(viewModel);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in EndShift");
            return OperationResponseBase.NotFound<ShiftR>($"Shift not found with id: {shiftId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in EndShift");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }

    public ShiftR GetAllShifts(bool onlyActive = true)
    {
        try
        {
            var shiftDataModels = _shiftBusinessLogic.GetAllShifts(onlyActive);
            var shiftViewModels = _mapper.Map<List<ShiftVM>>(shiftDataModels);
            return OperationResponseBase.OK<ShiftR, List<ShiftVM>>(shiftViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllShifts");
            return OperationResponseBase.InternalServerError<ShiftR>(ex.Message);
        }
    }
}
