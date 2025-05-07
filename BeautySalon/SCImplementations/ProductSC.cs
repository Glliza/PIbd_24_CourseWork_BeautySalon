using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using BeautySalon.Enums;
using AutoMapper;

namespace BeautySalon.SCImplementations;

internal class ProductSC : IProductSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public ProductSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDM>();

            // Ignore properties not present in DM (like IsDeleted) or handled separately
            cfg.CreateMap<ProductDM, Product>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            // IsDeleted is managed by SC logic, not mapped directly
            // Add Ignore for any navigation properties on the EF entity if you don't map them directly
            // .ForMember(dest => dest.ProductListItems, opt => opt.Ignore());
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<ProductDM>> GetList(bool onlyActive = true, string? name = null, int? stockQuantityBelow = null, ProductType? type = null)
    {
        try
        {
            var query = _dbContext.Products.AsQueryable();
            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (name is not null)
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (stockQuantityBelow is not null && stockQuantityBelow > 0)
            {
                query = query.Where(x => x.StockQuantity < stockQuantityBelow.Value);
            }
            if (type is not null)
            {
                query = query.Where(x => x.Type == type.Value);
                // Use EF Entity property name (assuming 'Type' property on EF Product)
            }

            // Execute the query asynchronously and map results to DM
            var productEntities = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ProductDM>>(productEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<ProductDM?> GetElementByID(string id)
    {
        try
        {
            var productEntity = await GetProductByID(id);
            return _mapper.Map<ProductDM>(productEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    // Implement GetElementByName method (async) [ ! ]
    // Note: Assumes Product Name is unique for active products based on your DbContext index config.
    // If not unique, this will return the first match.
    public async Task<ProductDM?> GetElementByName(string name)
    {
        try
        {
            // Find the active product entity by name
            var productEntity = await _dbContext.Products
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted);

            return _mapper.Map<ProductDM>(productEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task AddElement(ProductDM productDataModel)
    {
        try
        {
            productDataModel.Validate();

            var existingElement = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ID == productDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ProductEntityID", productDataModel.ID);
            }


            // Check if an active product with the same name already exists (based on unique index config)
            var existingProductByName = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name == productDataModel.Name && !x.IsDeleted);
            if (existingProductByName != null)
            {
                throw new ElementExistsException("ProductEntityID", productDataModel.ID);
            }

            // Map DM to Entity
            var productEntity = _mapper.Map<Product>(productDataModel);
            productEntity.IsDeleted = false;
            // Explicitly set IsDeleted flag to false when adding

            // Add the entity to the DbContext change tracker
            await _dbContext.Products.AddAsync(productEntity);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            // Check inner exception for specific database errors (e.g., unique index violation)
            // For PostgreSQL Npgsql, unique constraint violation often has SqlState '23505'
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                // Attempt to provide more detail based on the constraint name if available
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ProductEntityID", productDataModel.ID);
            }
            throw new StorageException(ex);
        }
        catch (ValidationException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (ElementExistsException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task UpdElement(ProductDM productDataModel)
    {
        try
        {
            productDataModel.Validate();

            var element = await GetProductByID(productDataModel.ID);
            if (element == null)
            {
                // If GetProductByID (which filters !IsDeleted) returns null, the element is not found or is deleted
                throw new ElementNotFoundException(productDataModel.ID);
            }

            // Prevent updating if soft-deleted
            if (element.IsDeleted)
            {
                throw new ElementNotFoundException(productDataModel.ID);
            }

            // Check if the updated name conflicts with another active product (if name was changed)
            if (element.Name != productDataModel.Name)
            {
                var existingProductByName = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name == productDataModel.Name && x.ID != productDataModel.ID && !x.IsDeleted);
                if (existingProductByName != null)
                {
                    throw new ElementExistsException("ProductEntityNAME", productDataModel.Name);
                }
            }
            _mapper.Map(productDataModel, element);


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ProductEntityID", productDataModel.ID);
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task DelElement(string id)
    {
        try
        {
            var element = await GetProductByID(id);
            if (element == null)
            {
                throw new ElementNotFoundException(id);
            }

            element.IsDeleted = true;

            // Note: Consider business rules for related entities (e.g., ProductListItems).
            // Should deleting a Product soft-delete its associated items on Receipts/Requests?
            // If yes, you would load related items here and set IsDeleted = true.
            // e.g., await _dbContext.ProductListItems.Where(item => item.ProductID == id).ForEachAsync(item => item.IsDeleted = true);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task RestoreElement(string id)
    {
        try
        {
            // Find the soft-deleted product entity to restore
            // Note: Need to find *including* deleted ones here and check if it *is* deleted
            var element = await GetAnyProductByID(id);

            if (element == null || !element.IsDeleted) // Check if found AND is currently deleted
            {
                throw new ElementNotFoundException(id);
            }

            // Check if restoring would create a name conflict with an existing active product
            var existingProductByName = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name == element.Name && x.ID != element.ID && !x.IsDeleted);
            if (existingProductByName != null)
            {
                throw new ElementExistsException("PrEntityNAME", element.Name);
            }

            // Restore the element
            element.IsDeleted = false;

            // Attach the entity if AsNoTracking was used to load it, and mark as modified
            _dbContext.Products.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;


            // Note: Consider cascading restore for related entities (e.g., ProductListItems) if they were soft-deleted
            // along with the product. This would require loading them and setting IsDeleted = false.

            // Save changes
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ProductEntityID", id);
            }
            throw new StorageException(ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task UpdateStockQuantityAsync(string productId, int quantityChange)
    {
        try
        {
            // Find the active product by ID for tracking
            var element = await _dbContext.Products.FirstOrDefaultAsync(x => x.ID == productId && !x.IsDeleted);

            if (element == null)
            {
                throw new ElementNotFoundException(productId);
            }

            // Update stock quantity
            element.StockQuantity += quantityChange;

            // Optional: Add validation to prevent negative stock if needed
            if (element.StockQuantity < 0)
            {
                _dbContext.ChangeTracker.Clear(); // Clear changes if validation fails before saving
                throw new ValidationException($"Product {productId}: Stock quantity cannot be negative after update.");
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
        catch (ValidationException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }


    // Helper method to get an active product entity by ID
    private Task<Product?> GetProductByID(string id)
    {
        return _dbContext.Products
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }

    // Helper method to get any product entity (including deleted) by ID
    private Task<Product?> GetAnyProductByID(string id)
    {
        return _dbContext.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}