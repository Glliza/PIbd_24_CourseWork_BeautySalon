using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations
{
    public class StaffAdapter : IStaffAdapter
    {
        private readonly IStaffBLC _staffBusinessLogic;
        private readonly ILogger<StaffAdapter> _logger;
        private readonly IMapper _mapper;

        public StaffAdapter(IStaffBLC staffBusinessLogic, ILogger<StaffAdapter> logger, IMapper mapper)
        {
            _staffBusinessLogic = staffBusinessLogic ?? throw new ArgumentNullException(nameof(staffBusinessLogic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private StaffR HandleNotFound(string message)
        {
            return OperationResponseBase.NotFound<StaffR>(message);
        }

        public StaffR GetUserByLogin(string login)
        {
            try
            {
                var staffDataModel = _staffBusinessLogic.GetStaffByFIO(login);
                if (staffDataModel == null)
                {
                    return HandleNotFound($"Staff not found with login: {login}");
                }
                var staffViewModel = _mapper.Map<StaffVM>(staffDataModel);
                return OperationResponseBase.OK<StaffR, StaffVM>(staffViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetUserByLogin");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetUserByLogin");
                return HandleNotFound($"Staff not found with login: {login}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetUserByLogin");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR RegisterWorker(StaffVM staffModel)
        {
            try
            {
                var staffDataModel = _mapper.Map<StaffDM>(staffModel);
                if (staffDataModel == null)
                {
                    _logger.LogError("Mapping failed from StaffViewModel to StaffDataModel in RegisterWorker");
                    return OperationResponseBase.BadRequest<StaffR>("Invalid staff data provided.");
                }
                _staffBusinessLogic.InsertStaff(staffDataModel);
                var registeredStaff = _staffBusinessLogic.GetStaffByFIO(staffModel.FIO);
                if (registeredStaff == null)
                {
                    return OperationResponseBase.NotFound<StaffR>($"Staff not found with login: {staffModel.FIO}");
                }
                var viewModel = _mapper.Map<StaffVM>(registeredStaff);
                if (viewModel == null)
                {
                    _logger.LogError("Mapping failed from StaffDataModel to StaffViewModel in RegisterWorker");
                    return OperationResponseBase.InternalServerError<StaffR>("Failed to map registered staff data.");
                }
                return OperationResponseBase.OK<StaffR, StaffVM>(viewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in RegisterWorker");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty. Please ensure all required fields are provided.");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException in RegisterWorker");
                return OperationResponseBase.BadRequest<StaffR>($"Incorrect data transmitted: {ex.Message}. Please check the format and values of your input data.");
            }
            catch (ElementExistsException ex)
            {
                _logger.LogError(ex, "ElementExistsException in RegisterWorker");
                return OperationResponseBase.BadRequest<StaffR>($"A worker with this FIO already exists. Please use a different FIO or log in if you already have an account.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RegisterWorker");
                return OperationResponseBase.InternalServerError<StaffR>("An unexpected error occurred while processing your request. Please try again later.");
            }
        }

        public StaffR ChangeStaffInfo(StaffVM workerModel)
        {
            try
            {
                var staffDataModel = _mapper.Map<StaffDM>(workerModel);
                _staffBusinessLogic.UpdateStaff(staffDataModel);
                return OperationResponseBase.OK<StaffR, StaffVM>(workerModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in ChangeStaffInfo");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in ChangeStaffInfo");
                return OperationResponseBase.NotFound<StaffR>($"Customer not found with id: {workerModel.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ChangeStaffInfo");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR RemoveStaff(string id)
        {
            try
            {
                _staffBusinessLogic.DeleteStaff(id);
                return OperationResponseBase.NoContent<StaffR>();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in RemoveCustomer");
                return OperationResponseBase.NotFound<StaffR>($"Customer not found with id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RemoveCustomer");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR GetStaffByFIO(string fio)
        {
            try
            {
                var staffDataModel = _staffBusinessLogic.GetStaffByFIO(fio);
                if (staffDataModel == null)
                {
                    return HandleNotFound($"Staff not found with FIO: {fio}");
                }
                var staffViewModel = _mapper.Map<StaffVM>(staffDataModel);
                return OperationResponseBase.OK<StaffR, StaffVM>(staffViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetStaffByFIO");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetStaffByFIO");
                return HandleNotFound($"Staff not found with FIO: {fio}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetStaffByFIO");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR GetStaffById(string id)
        {
            try
            {
                var staffDataModel = _staffBusinessLogic.GetStaffById(id);
                if (staffDataModel == null)
                {
                    return HandleNotFound($"Staff not found with id: {id}");
                }
                var staffViewModel = _mapper.Map<StaffVM>(staffDataModel);
                return OperationResponseBase.OK<StaffR, StaffVM>(staffViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetStaffById");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetStaffById");
                return HandleNotFound($"Staff not found with id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetStaffById");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR CreateStaff(StaffVM staffModel)
        {
            try
            {
                var staffDataModel = _mapper.Map<StaffDM>(staffModel);
                if (staffDataModel == null)
                {
                    _logger.LogError("Mapping failed from StaffViewModel to StaffDataModel in CreateStaff");
                    return OperationResponseBase.BadRequest<StaffR>("Invalid staff data provided.");
                }
                _staffBusinessLogic.InsertStaff(staffDataModel);
                var createdStaff = _staffBusinessLogic.GetStaffByFIO(staffModel.FIO);
                if (createdStaff == null)
                {
                    return OperationResponseBase.NotFound<StaffR>($"Staff not found with FIO: {staffModel.FIO}");
                }
                var viewModel = _mapper.Map<StaffVM>(createdStaff);
                if (viewModel == null)
                {
                    _logger.LogError("Mapping failed from StaffDataModel to StaffViewModel in CreateStaff");
                    return OperationResponseBase.InternalServerError<StaffR>("Failed to map created staff data.");
                }
                return OperationResponseBase.OK<StaffR, StaffVM>(viewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in CreateStaff");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException in CreateStaff");
                return OperationResponseBase.BadRequest<StaffR>($"Incorrect data transmitted: {ex.Message}");
            }
            catch (ElementExistsException ex)
            {
                _logger.LogError(ex, "ElementExistsException in CreateStaff");
                return OperationResponseBase.BadRequest<StaffR>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in CreateStaff");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR UpdateStaff(StaffVM staffModel)
        {
            try
            {
                var staffDataModel = _mapper.Map<StaffDM>(staffModel);
                _staffBusinessLogic.UpdateStaff(staffDataModel);
                return OperationResponseBase.OK<StaffR, StaffVM>(staffModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in UpdateStaff");
                return OperationResponseBase.BadRequest<StaffR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in UpdateStaff");
                return OperationResponseBase.NotFound<StaffR>($"Staff not found with id: {staffModel.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateStaff");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }

        public StaffR GetAllStaff(bool onlyActive = true)
        {
            try
            {
                var staffDataModels = _staffBusinessLogic.GetAllStaff(onlyActive);
                var staffViewModels = _mapper.Map<List<StaffVM>>(staffDataModels);
                return OperationResponseBase.OK<StaffR, List<StaffVM>>(staffViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAllStaff");
                return OperationResponseBase.InternalServerError<StaffR>(ex.Message);
            }
        }
    }
}