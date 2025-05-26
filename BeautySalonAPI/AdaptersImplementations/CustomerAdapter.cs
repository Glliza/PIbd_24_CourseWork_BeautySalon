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
    public class CustomerAdapter : ICustomerAdapter
    {
        private readonly ICustomerBLC _customerBusinessLogic;
        private readonly ILogger<CustomerAdapter> _logger;
        private readonly IMapper _mapper;

        public CustomerAdapter(ICustomerBLC customerBusinessLogic, ILogger<CustomerAdapter> logger, IMapper mapper)
        {
            _customerBusinessLogic = customerBusinessLogic ?? throw new ArgumentNullException(nameof(customerBusinessLogic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private CustomerR HandleNotFound(string message)
        {
            return OperationResponseBase.NotFound<CustomerR>(message);
        }

        public CustomerR GetCustomerById(string id)
        {
            try
            {
                var customerDataModel = _customerBusinessLogic.GetCustomerById(id);
                if (customerDataModel == null)
                {
                    return HandleNotFound($"Customer not found with id: {id}");
                }
                var customerViewModel = _mapper.Map<CustomerVM>(customerDataModel);
                return OperationResponseBase.OK<CustomerR, CustomerVM>(customerViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetCustomerById");
                return OperationResponseBase.BadRequest<CustomerR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetCustomerById");
                return HandleNotFound($"Customer not found with id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetCustomerById");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }

        public CustomerR GetCustomerByPhoneNumber(string phoneNumber)
        {
            try
            {
                var customerDataModel = _customerBusinessLogic.GetCustomerByPhoneNumber(phoneNumber);
                if (customerDataModel == null)
                {
                    return HandleNotFound($"Customer not found with phone number: {phoneNumber}");
                }
                var customerViewModel = _mapper.Map<CustomerVM>(customerDataModel);
                return OperationResponseBase.OK<CustomerR, CustomerVM>(customerViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetCustomerByPhoneNumber");
                return OperationResponseBase.BadRequest<CustomerR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetCustomerByPhoneNumber");
                return HandleNotFound($"Customer not found with phone number: {phoneNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetCustomerByPhoneNumber");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }

        public CustomerR RegisterCustomer(CustomerVM customerModel)
        {
            try
            {
                var customerDataModel = _mapper.Map<CustomerDM>(customerModel);
                if (customerDataModel == null)
                {
                    _logger.LogError("Mapping failed from CustomerViewModel to CustomerDataModel in RegisterCustomer");
                    return OperationResponseBase.BadRequest<CustomerR>("Invalid customer data provided.");
                }
                _customerBusinessLogic.InsertCustomer(customerDataModel);
                var registeredCustomer = _customerBusinessLogic.GetCustomerByPhoneNumber(customerModel.PhoneNumber);
                if (registeredCustomer == null)
                {
                    return OperationResponseBase.NotFound<CustomerR>($"Customer not found with phone number: {customerModel.PhoneNumber}");
                }
                var viewModel = _mapper.Map<CustomerVM>(registeredCustomer);
                if (viewModel == null)
                {
                    _logger.LogError("Mapping failed from CustomerDataModel to CustomerViewModel in RegisterCustomer");
                    return OperationResponseBase.InternalServerError<CustomerR>("Failed to map registered customer data.");
                }
                return OperationResponseBase.OK<CustomerR, CustomerVM>(viewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in RegisterCustomer");
                return OperationResponseBase.BadRequest<CustomerR>("Data is empty. Please ensure all required fields are provided.");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException in RegisterCustomer");
                return OperationResponseBase.BadRequest<CustomerR>($"Incorrect data transmitted: {ex.Message}. Please check the format and values of your input data.");
            }
            catch (ElementExistsException ex)
            {
                _logger.LogError(ex, "ElementExistsException in RegisterCustomer");
                return OperationResponseBase.BadRequest<CustomerR>("A customer with this phone number already exists. Please use a different phone number or log in if you already have an account.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RegisterCustomer");
                return OperationResponseBase.InternalServerError<CustomerR>("An unexpected error occurred while processing your request. Please try again later.");
            }
        }

        public CustomerR ChangeCustomerInfo(CustomerVM customerModel)
        {
            try
            {
                var customerDataModel = _mapper.Map<CustomerDM>(customerModel);
                _customerBusinessLogic.UpdateCustomer(customerDataModel);
                return OperationResponseBase.OK<CustomerR, CustomerVM>(customerModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in ChangeCustomerInfo");
                return OperationResponseBase.BadRequest<CustomerR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in ChangeCustomerInfo");
                return OperationResponseBase.NotFound<CustomerR>($"Customer not found with id: {customerModel.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ChangeCustomerInfo");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }

        public CustomerR RemoveCustomer(string id)
        {
            try
            {
                _customerBusinessLogic.DeleteCustomer(id);
                return OperationResponseBase.NoContent<CustomerR>();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in RemoveCustomer");
                return OperationResponseBase.NotFound<CustomerR>($"Customer not found with id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RemoveCustomer");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }

        public CustomerR GetCustomerByFIO(string fio)
        {
            try
            {
                var customerDataModel = _customerBusinessLogic.GetFilteredCustomers(fio: fio).FirstOrDefault();
                if (customerDataModel == null)
                {
                    return HandleNotFound($"Customer not found with FIO: {fio}");
                }
                var customerViewModel = _mapper.Map<CustomerVM>(customerDataModel);
                return OperationResponseBase.OK<CustomerR, CustomerVM>(customerViewModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException in GetCustomerByFIO");
                return OperationResponseBase.BadRequest<CustomerR>("Data is empty");
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, "ElementNotFoundException in GetCustomerByFIO");
                return HandleNotFound($"Customer not found with FIO: {fio}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetCustomerByFIO");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }

        public CustomerR GetAllCustomers(bool onlyActive = true)
        {
            try
            {
                var customerDataModels = _customerBusinessLogic.GetAllCustomers(onlyActive);
                var customerViewModels = _mapper.Map<List<CustomerVM>>(customerDataModels);
                return OperationResponseBase.OK<CustomerR, List<CustomerVM>>(customerViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAllCustomers");
                return OperationResponseBase.InternalServerError<CustomerR>(ex.Message);
            }
        }
    }
}
