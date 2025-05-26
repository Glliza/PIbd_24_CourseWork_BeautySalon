using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class ProductAdapter : IProductAdapter
{
    private readonly IProductBLC _productBusinessLogic;
    private readonly ILogger<ProductAdapter> _logger;
    private readonly IMapper _mapper;

    public ProductAdapter(IProductBLC productBusinessLogic, ILogger<ProductAdapter> logger, IMapper mapper)
    {
        _productBusinessLogic = productBusinessLogic ?? throw new ArgumentNullException(nameof(productBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    private ProductR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<ProductR>(message);
    }

    public ProductR GetProductById(string id)
    {
        try
        {
            var productDataModel = _productBusinessLogic.GetProductById(id);
            if (productDataModel == null)
            {
                return HandleNotFound($"Product not found with id: {id}");
            }
            var productViewModel = _mapper.Map<ProductVM>(productDataModel);
            return OperationResponseBase.OK<ProductR, ProductVM>(productViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetProductById");
            return OperationResponseBase.BadRequest<ProductR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetProductById");
            return HandleNotFound($"Product not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetProductById");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }

    public ProductR GetProductByName(string name)
    {
        try
        {
            var productDataModel = _productBusinessLogic.GetProductByName(name);
            if (productDataModel == null)
            {
                return HandleNotFound($"Product not found with name: {name}");
            }
            var productViewModel = _mapper.Map<ProductVM>(productDataModel);
            return OperationResponseBase.OK<ProductR, ProductVM>(productViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetProductByName");
            return OperationResponseBase.BadRequest<ProductR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetProductByName");
            return HandleNotFound($"Product not found with name: {name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetProductByName");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }

    public ProductR CreateProduct(ProductVM productModel)
    {
        try
        {
            var productDataModel = _mapper.Map<ProductDM>(productModel);
            if (productDataModel == null)
            {
                _logger.LogError("Mapping failed from ProductViewModel to ProductDataModel in CreateProduct");
                return OperationResponseBase.BadRequest<ProductR>("Invalid product data provided.");
            }
            _productBusinessLogic.InsertProduct(productDataModel);
            var createdProduct = _productBusinessLogic.GetProductById(productDataModel.ID);
            if (createdProduct == null)
            {
                return OperationResponseBase.NotFound<ProductR>($"Product not found with id: {productModel.Id}");
            }
            var viewModel = _mapper.Map<ProductVM>(createdProduct);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ProductDataModel to ProductViewModel in CreateProduct");
                return OperationResponseBase.InternalServerError<ProductR>("Failed to map created product data.");
            }
            return OperationResponseBase.OK<ProductR, ProductVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateProduct");
            return OperationResponseBase.BadRequest<ProductR>("Data is empty. Please ensure all required fields are provided.");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateProduct");
            return OperationResponseBase.BadRequest<ProductR>($"Incorrect data transmitted: {ex.Message}. Please check the format and values of your input data.");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException in CreateProduct");
            return OperationResponseBase.BadRequest<ProductR>("A product with this name already exists. Please use a different name or update the existing product.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateProduct");
            return OperationResponseBase.InternalServerError<ProductR>("An unexpected error occurred while processing your request. Please try again later.");
        }
    }

    public ProductR UpdateProduct(ProductVM productModel)
    {
        try
        {
            var productDataModel = _mapper.Map<ProductDM>(productModel);
            _productBusinessLogic.UpdateProduct(productDataModel);
            return OperationResponseBase.OK<ProductR, ProductVM>(productModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateProduct");
            return OperationResponseBase.BadRequest<ProductR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateProduct");
            return OperationResponseBase.NotFound<ProductR>($"Product not found with id: {productModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateProduct");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }

    public ProductR DeleteProduct(string id)
    {
        try
        {
            _productBusinessLogic.DeleteProduct(id);
            return OperationResponseBase.NoContent<ProductR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteProduct");
            return OperationResponseBase.NotFound<ProductR>($"Product not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteProduct");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }

    public ProductR UpdateStockQuantity(string productId, int quantityChange)
    {
        try
        {
            _productBusinessLogic.UpdateStockQuantity(productId, quantityChange);
            return OperationResponseBase.OK<ProductR, string>("Stock quantity updated successfully"); // Consider returning more specific data if needed
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateStockQuantity");
            return OperationResponseBase.NotFound<ProductR>($"Product not found with id: {productId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateStockQuantity");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }

    public ProductR GetAllProducts(bool onlyActive = true)
    {
        try
        {
            var productDataModels = _productBusinessLogic.GetAllProducts(onlyActive);
            var productViewModels = _mapper.Map<List<ProductVM>>(productDataModels);
            return OperationResponseBase.OK<ProductR, List<ProductVM>>(productViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllProducts");
            return OperationResponseBase.InternalServerError<ProductR>(ex.Message);
        }
    }
}