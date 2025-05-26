using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using System.Linq; // For FirstOrDefault
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class ReceiptAdapter : IReceiptAdapter
{
    private readonly IReceiptBLC _receiptBusinessLogic;
    private readonly ILogger<ReceiptAdapter> _logger;
    private readonly IMapper _mapper;
    private readonly IShiftBLC _shiftBusinessLogic;

    public ReceiptAdapter(IReceiptBLC receiptBusinessLogic, ILogger<ReceiptAdapter> logger, IMapper mapper, IShiftBLC shiftBusinessLogic)
    {
        _receiptBusinessLogic = receiptBusinessLogic ?? throw new ArgumentNullException(nameof(receiptBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _shiftBusinessLogic = shiftBusinessLogic ?? throw new ArgumentNullException(nameof(shiftBusinessLogic));
    }

    private ReceiptR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<ReceiptR>(message);
    }

    public ReceiptR GetReceiptById(string id)
    {
        try
        {
            var receiptDataModel = _receiptBusinessLogic.GetReceiptById(id);
            if (receiptDataModel == null)
            {
                return HandleNotFound($"Receipt not found with id: {id}");
            }
            var receiptViewModel = _mapper.Map<ReceiptVM>(receiptDataModel);
            return OperationResponseBase.OK<ReceiptR, ReceiptVM>(receiptViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetReceiptById");
            return OperationResponseBase.BadRequest<ReceiptR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetReceiptById");
            return HandleNotFound($"Receipt not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetReceiptById");
            return OperationResponseBase.InternalServerError<ReceiptR>(ex.Message);
        }
    }

    public ReceiptR CreateReceipt(ReceiptVM receiptModel)
    {
        try
        {
            var receiptDataModel = _mapper.Map<ReceiptDM>(receiptModel);
            if (receiptDataModel == null)
            {
                _logger.LogError("Mapping failed from ReceiptViewModel to ReceiptDataModel in CreateReceipt");
                return OperationResponseBase.BadRequest<ReceiptR>("Invalid receipt data provided.");
            }
            // Get the active shift for the staff and extract the cash box ID
            var activeShift = _shiftBusinessLogic.GetActiveShiftForStaff(receiptModel.StaffID);
            if (activeShift == null)
            {
                _logger.LogError("No active shift found for staff with id: {StaffId}", receiptModel.StaffID);
                return OperationResponseBase.BadRequest<ReceiptR>("No active shift found for the provided staff.");
            }

            string cashBoxId = activeShift.CashBoxID;
            _receiptBusinessLogic.InsertReceipt(receiptDataModel, cashBoxId);
            var createdReceipt = _receiptBusinessLogic.GetReceiptById(receiptDataModel.ID);
            if (createdReceipt == null)
            {
                return OperationResponseBase.NotFound<ReceiptR>($"Receipt not found with id: {receiptModel.Id}");
            }
            var viewModel = _mapper.Map<ReceiptVM>(createdReceipt);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ReceiptDataModel to ReceiptViewModel in CreateReceipt");
                return OperationResponseBase.InternalServerError<ReceiptR>("Failed to map created receipt data.");
            }
            return OperationResponseBase.OK<ReceiptR, ReceiptVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateReceipt");
            return OperationResponseBase.BadRequest<ReceiptR>("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateReceipt");
            return OperationResponseBase.BadRequest<ReceiptR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateReceipt");
            return OperationResponseBase.InternalServerError<ReceiptR>(ex.Message);
        }
    }

    public ReceiptR UpdateReceipt(ReceiptVM receiptModel)
    {
        try
        {
            var receiptDataModel = _mapper.Map<ReceiptDM>(receiptModel);
            _receiptBusinessLogic.UpdateReceipt(receiptDataModel);
            return OperationResponseBase.OK<ReceiptR, ReceiptVM>(receiptModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateReceipt");
            return OperationResponseBase.BadRequest<ReceiptR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateReceipt");
            return OperationResponseBase.NotFound<ReceiptR>($"Receipt not found with id: {receiptModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateReceipt");
            return OperationResponseBase.InternalServerError<ReceiptR>(ex.Message);
        }
    }

    public ReceiptR DeleteReceipt(string id)
    {
        try
        {
            _receiptBusinessLogic.DeleteReceipt(id);
            return OperationResponseBase.NoContent<ReceiptR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteReceipt");
            return OperationResponseBase.NotFound<ReceiptR>($"Receipt not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteReceipt");
            return OperationResponseBase.InternalServerError<ReceiptR>(ex.Message);
        }
    }

    public ReceiptR GetAllReceipts(bool onlyActive = true)
    {
        try
        {
            var receiptDataModels = _receiptBusinessLogic.GetAllReceipts(onlyActive);
            var receiptViewModels = _mapper.Map<List<ReceiptVM>>(receiptDataModels);
            return OperationResponseBase.OK<ReceiptR, List<ReceiptVM>>(receiptViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllReceipts");
            return OperationResponseBase.InternalServerError<ReceiptR>(ex.Message);
        }
    }
}
