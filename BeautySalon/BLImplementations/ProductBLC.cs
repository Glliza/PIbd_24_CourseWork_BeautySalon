using BeautySalon.BusinessLogicContracts;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.Enums;
using System.Text.Json;

namespace BeautySalon.BLImplementations;

internal class ProductBLC(IProductSC productStorageContract, ILogger logger)
    : IProductBLC
{
    private readonly ILogger _logger = logger;
    private readonly IProductSC _productStorageContract = productStorageContract;

    public List<ProductDM> GetAllProducts(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllProducts params: {onlyActive}", onlyActive);
        return _productStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
            ?? throw new NullListException();
    }

    public List<ProductDM> GetFilteredProducts(
        string? name = null,
        int? stockQuantityBelow = null,
        ProductType? type = null)
    {
        _logger.LogInformation("GetFilteredProducts params: {name}, {stockQuantityBelow}, {type}",
            name, stockQuantityBelow, type);

        return _productStorageContract.GetList(true, name, stockQuantityBelow, type)
            .GetAwaiter().GetResult() ?? throw new NullListException();
    }

    public ProductDM GetProductById(string id)
    {
        _logger.LogInformation("GetProductById for {id}", id);

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Product ID is not a valid GUID");
        }

        var result = _productStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public ProductDM GetProductByName(string name)
    {
        _logger.LogInformation("GetProductByName for {name}", name);

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var result = _productStorageContract.GetElementByName(name).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(name);
    }

    public void InsertProduct(ProductDM productDataModel)
    {
        _logger.LogInformation("New product data: {json}", JsonSerializer.Serialize(productDataModel));

        ArgumentNullException.ThrowIfNull(productDataModel);
        productDataModel.Validate();

        _productStorageContract.AddElement(productDataModel).GetAwaiter().GetResult();
    }

    public void UpdateProduct(ProductDM productDataModel)
    {
        _logger.LogInformation("Update product data: {json}", JsonSerializer.Serialize(productDataModel));

        ArgumentNullException.ThrowIfNull(productDataModel);
        productDataModel.Validate();

        _productStorageContract.UpdElement(productDataModel).GetAwaiter().GetResult();
    }

    public void DeleteProduct(string id)
    {
        _logger.LogInformation("Delete product by id: {id}", id);

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Product ID is not a valid GUID");
        }

        _productStorageContract.DelElement(id).GetAwaiter().GetResult();
    }

    public void UpdateStockQuantity(string productId, int quantityChange)
    {
        _logger.LogInformation("Update stock for product {productId} with change {quantityChange}",
            productId, quantityChange);

        if (string.IsNullOrEmpty(productId))
        {
            throw new ArgumentNullException(nameof(productId));
        }

        if (!productId.IsGuid())
        {
            throw new ValidationException("Product ID is not a valid GUID");
        }

        if (quantityChange == 0)
        {
            throw new ValidationException("Quantity change cannot be zero");
        }

        _productStorageContract.UpdateStockQuantityAsync(productId, quantityChange)
            .GetAwaiter().GetResult();
    }
}