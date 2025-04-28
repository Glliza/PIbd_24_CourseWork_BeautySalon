using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using BeautySalon.Enums;
using AutoMapper;

namespace BeautySalon.Implementations;

internal class RequestSC : IRequestSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public RequestSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;

        var config = new MapperConfiguration(cfg =>
        {
            // Mapping from EF Entity to Core DM
            cfg.CreateMap<Request, RequestDM>(); // Main Request mapping
            cfg.CreateMap<ProductListItem, ProductListItemDM>(); // Product List Item mapping
            cfg.CreateMap<ServiceListItem, ServiceListItemDM>(); // Service List Item mapping

            // Mapping from Core DM to EF Entity
            // Ensure mapping handles the nested lists and ignores properties like IsDeleted managed by SC
            cfg.CreateMap<RequestDM, Request>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
               .ForMember(dest => dest.ProductItems, opt => opt.Ignore())
               .ForMember(dest => dest.ServiceItems, opt => opt.Ignore());

            // Map from DM list item to EF list item entity, ignore parent FKs and IsDeleted
            cfg.CreateMap<ProductListItemDM, ProductListItem>()
                .ForMember(dest => dest.ParentRequestID, opt => opt.Ignore())
                .ForMember(dest => dest.ParentReceiptID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            cfg.CreateMap<ServiceListItemDM, ServiceListItem>()
               .ForMember(dest => dest.ParentRequestID, opt => opt.Ignore())
               .ForMember(dest => dest.ParentVisitID, opt => opt.Ignore())
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        });
        _mapper = new Mapper(config); // Create Mapper instance
    }

    // Implement GetList method (async)
    public async Task<List<RequestDM>> GetList(bool onlyActive = true, string? customerID = null, OrderStatus? status = null, DateTime? fromDateCreated = null, DateTime? toDateCreated = null)
    {
        try
        {
            // !!!
            var query = _dbContext.Requests
                                  .Include(r => r.Customer)
                                  // Decide if you need to include items for list view - often not needed for summary list
                                  // .Include(r => r.ProductItems)
                                  // .Include(r => r.ServiceItems)
                                  .AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (customerID is not null)
            {
                query = query.Where(x => x.CustomerID == customerID);
            }
            if (status is not null)
            {
                query = query.Where(x => x.Status == status.Value); // Use EF Entity property name
            }
            if (fromDateCreated is not null)
            {
                query = query.Where(x => x.DateCreated >= fromDateCreated.Value);
            }
            if (toDateCreated is not null)
            {
                // Use AddDays(1) and < endDate for inclusive date range query common pattern
                query = query.Where(x => x.DateCreated < toDateCreated.Value.AddDays(1));
            }


            var requestEntities = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<RequestDM>>(requestEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<RequestDM?> GetElementByID(string id)
    {
        try
        {
            // Find the active request entity by ID, including related Customer
            var requestEntity = await GetRequestByID(id); // Use async helper method

            return _mapper.Map<RequestDM>(requestEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<RequestDM?> GetRequestWithItemsAsync(string id)
    {
        try
        {
            // Find the active request entity by ID, and INCLUDE its items
            var requestEntity = await _dbContext.Requests
                                                .Include(r => r.Customer) // Include related Customer
                                                .Include(r => r.ProductItems) // Include ProductListItems
                                                    .ThenInclude(pli => pli.Product) // Optionally include the Product details for each item
                                                .Include(r => r.ServiceItems) // Include ServiceListItems
                                                    .ThenInclude(sli => sli.Service) // Optionally include the Service details for each item
                                                .AsNoTracking() // Use AsNoTracking
                                                .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted); // Find active

            return _mapper.Map<RequestDM>(requestEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task AddElement(RequestDM requestDataModel)
    {
        try
        {
            requestDataModel.Validate();

            var existingElement = await _dbContext.Requests.AsNoTracking().FirstOrDefaultAsync(x => x.ID == requestDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("RequestEntityID", requestDataModel.ID);
            }

            // Validate existence of related Customer
            var customerExists = await _dbContext.Customers.AsNoTracking().AnyAsync(c => c.ID == requestDataModel.CustomerID && !c.IsDeleted);
            if (!customerExists) throw new ElementNotFoundException(requestDataModel.CustomerID);

            // Optional: Validate existence of Products and Services referenced by list items
            // Loop through items and check if ProductID/ServiceID exist in DB
            foreach (var productItemDm in requestDataModel.ProductItems)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID);
            }
            foreach (var serviceItemDm in requestDataModel.ServiceItems)
            {
                var serviceExists = await _dbContext.Services.AsNoTracking().AnyAsync(s => s.ID == serviceItemDm.ServiceID && !s.IsDeleted);
                if (!serviceExists) throw new ElementNotFoundException(serviceItemDm.ServiceID);
            }


            // Map DM to EF Entity (header only initially, items handled separately or via navigation)
            var requestEntity = _mapper.Map<Request>(requestDataModel);
            requestEntity.IsDeleted = false; // Explicitly set IsDeleted flag to false

            // Manually map and add list items, setting the parent FK and IsDeleted
            requestEntity.ProductItems = requestDataModel.ProductItems
                .Select(itemDm => {
                    var itemEntity = _mapper.Map<ProductListItem>(itemDm);
                    itemEntity.ParentRequestID = requestEntity.ID; // Set the FK back to the parent request
                    itemEntity.IsDeleted = false; // Items are not deleted initially
                    return itemEntity;
                }).ToList();

            requestEntity.ServiceItems = requestDataModel.ServiceItems
               .Select(itemDm => {
                   var itemEntity = _mapper.Map<ServiceListItem>(itemDm);
                   itemEntity.ParentRequestID = requestEntity.ID; // Set the FK back to the parent request
                                                                  // itemEntity.ParentVisitID = null;
                                                                  // Explicitly set other potential parent FKs to null
                   itemEntity.IsDeleted = false; // Items are not deleted initially
                   return itemEntity;
               }).ToList();


            // Add the main Request entity. EF Core will handle cascading add for the items if mapped correctly.
            await _dbContext.Requests.AddAsync(requestEntity);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";

                if (sqlState == "23505") // Unique constraint violation
                {
                    throw new ElementExistsException("RequestEntityName", constraintName);
                }
                else if (sqlState == "23503") // FK violation
                {
                    throw new StorageException(ex);
                }
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task UpdElement(RequestDM requestDataModel)
    {
        try
        {
            requestDataModel.Validate();

            // Find the existing *active* entity by ID, including items for comparison/update
            var element = await _dbContext.Requests
                                          .Include(r => r.ProductItems)
                                          .Include(r => r.ServiceItems)
                                          .FirstOrDefaultAsync(x => x.ID == requestDataModel.ID && !x.IsDeleted); // Find active and *track* it

            if (element == null)
            {
                throw new ElementNotFoundException(requestDataModel.ID);
            }


            // Optional: Validate existence of updated related Customer if ID changes (less likely)
            // if (element.CustomerID != requestDataModel.CustomerID) { /* check new CustomerID */ }

            // Optional: Validate existence of Products and Services referenced by updated/new list items
            foreach (var productItemDm in requestDataModel.ProductItems)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID);
            }
            foreach (var serviceItemDm in requestDataModel.ServiceItems)
            {
                var serviceExists = await _dbContext.Services.AsNoTracking().AnyAsync(s => s.ID == serviceItemDm.ServiceID && !s.IsDeleted);
                if (!serviceExists) throw new ElementNotFoundException(serviceItemDm.ServiceID);
            }

            _mapper.Map(requestDataModel, element); // AutoMapper updates header properties

            // --- Update Nested Collections (ProductItems and ServiceItems) ---
            // This is a common pattern: remove old items not in the new list, add new items, update existing items.
            // Requires loading the existing items first (done above with .Include())

            // Handle Product Items
            var currentProductItemIds = element.ProductItems.Select(item => item.ID).ToList();
            var newItemProductItemIds = requestDataModel.ProductItems.Where(itemDm => !currentProductItemIds.Contains(itemDm.ProductID)).Select(itemDm => itemDm.ProductID).ToList();
            var updatedProductItemIds = requestDataModel.ProductItems.Where(itemDm => currentProductItemIds.Contains(itemDm.ProductID)).Select(itemDm => itemDm.ProductID).ToList();
            var removedProductItems = element.ProductItems.Where(itemEntity => !requestDataModel.ProductItems.Any(itemDm => itemDm.ProductID == itemEntity.ID)).ToList();

            // Remove items that are in the DB but not in the incoming DM list
            foreach (var itemToRemove in removedProductItems)
            {
                // Option 1: Hard Delete the item
                _dbContext.ProductListItems.Remove(itemToRemove); // Mark for deletion

                // Option 2: Soft Delete the item
                // itemToRemove.IsDeleted = true; // Requires loading items and setting IsDeleted
            }

            // Add new items that are in the incoming DM list but not in the DB
            foreach (var newItemDm in requestDataModel.ProductItems.Where(itemDm => newItemProductItemIds.Contains(itemDm.ProductID)))
            {
                var newItemEntity = _mapper.Map<ProductListItem>(newItemDm);
                newItemEntity.ParentRequestID = element.ID; // Set the FK back to the parent
                                                            // newItemEntity.ParentReceiptID = null;
                                                            // Explicitly set other potential parent FKs to null
                newItemEntity.IsDeleted = false; // New items are not deleted
                element.ProductItems.Add(newItemEntity); // Add to the collection, EF tracks it
            }

            // Update existing items that are in both the DB and the incoming DM list
            foreach (var updatedItemDm in requestDataModel.ProductItems.Where(itemDm => updatedProductItemIds.Contains(itemDm.ProductID)))
            {
                var existingItemEntity = element.ProductItems.FirstOrDefault(item => item.ID == updatedItemDm.ProductID);
                if (existingItemEntity != null)
                {
                    _mapper.Map(updatedItemDm, existingItemEntity); // Map changes onto existing entity
                    // Ensure IsDeleted is false if restoring via update - depends on scenario
                    // existingItemEntity.IsDeleted = false;
                }
            }


            // Repeat the same logic for Service Items
            var currentServiceItemIds = element.ServiceItems.Select(item => item.ID).ToList();
            var newItemServiceItemIds = requestDataModel.ServiceItems.Where(itemDm => !currentServiceItemIds.Contains(itemDm.ServiceID)).Select(itemDm => itemDm.ServiceID).ToList();
            var updatedServiceItemIds = requestDataModel.ServiceItems.Where(itemDm => currentServiceItemIds.Contains(itemDm.ServiceID)).Select(itemDm => itemDm.ServiceID).ToList();
            var removedServiceItems = element.ServiceItems.Where(itemEntity => !requestDataModel.ServiceItems.Any(itemDm => itemDm.ServiceID == itemEntity.ID)).ToList();

            // Remove items
            foreach (var itemToRemove in removedServiceItems) { _dbContext.ServiceListItems.Remove(itemToRemove); /* Or Soft Delete */ }

            // Add new items
            foreach (var newItemDm in requestDataModel.ServiceItems.Where(itemDm => newItemServiceItemIds.Contains(itemDm.ServiceID)))
            {
                var newItemEntity = _mapper.Map<ServiceListItem>(newItemDm);
                newItemEntity.ParentRequestID = element.ID; // Set the FK back
                                                            // newItemEntity.ParentVisitID = null; // Explicitly set other potential parent FKs to null
                newItemEntity.IsDeleted = false;
                element.ServiceItems.Add(newItemEntity);
            }

            // Update existing items
            foreach (var updatedItemDm in requestDataModel.ServiceItems.Where(itemDm => updatedServiceItemIds.Contains(itemDm.ServiceID)))
            {
                var existingItemEntity = element.ServiceItems.FirstOrDefault(item => item.ID == updatedItemDm.ServiceID);
                if (existingItemEntity != null)
                {
                    _mapper.Map(updatedItemDm, existingItemEntity);
                    // existingItemEntity.IsDeleted = false;
                }
            }

            // Save changes to the database. EF Core will detect changes to 'element' and its tracked children.
            await _dbContext.SaveChangesAsync();
        }
        // Catch specific exceptions for better handling
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";

                if (sqlState == "23505") // Unique constraint violation
                {
                    throw new ElementExistsException("RequestEntityID", requestDataModel.ID);
                }
                else if (sqlState == "23503") // FK violation
                {
                    throw new StorageException(ex);
                }
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task DelElement(string id)
    {
        try
        {
            // Find the active request entity to soft delete, including its items for cascading
            var element = await _dbContext.Requests
                                          .Include(r => r.ProductItems)
                                          .Include(r => r.ServiceItems)
                                          .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted); // Find active and *track* it

            if (element == null)
            {
                throw new ElementNotFoundException(id);
            }

            // Perform soft delete on the request
            element.IsDeleted = true;

            // Perform cascading soft delete on the items
            foreach (var item in element.ProductItems)
            {
                item.IsDeleted = true;
            }
            foreach (var item in element.ServiceItems)
            {
                item.IsDeleted = true;
            }

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
            throw; // Re-throw ElementNotFoundException directly
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
            // Find the soft-deleted request entity to restore, including its items
            // Note: Need to find *including* deleted ones here and check if it *is* deleted
            var element = await _dbContext.Requests
                                          .Include(r => r.ProductItems)
                                          .Include(r => r.ServiceItems)
                                          .FirstOrDefaultAsync(x => x.ID == id && x.IsDeleted); // Find deleted and *track* it

            if (element == null || !element.IsDeleted) // Check if found AND is currently deleted
            {
                throw new ElementNotFoundException(id);
            }

            // Optional: Check for unique constraint conflicts upon restoration (e.g., if a unique index exists on non-deleted requests)
            // This check is typically handled by the DbUpdateException on SaveChanges, but can be added here for earlier feedback if needed.

            // Restore the element
            element.IsDeleted = false;

            // Restore cascading soft-deleted items (only those that were deleted with this request)
            foreach (var item in element.ProductItems.Where(item => item.IsDeleted))
            {
                // Add more granular check if items can be deleted independently of the parent
                item.IsDeleted = false;
            }
            foreach (var item in element.ServiceItems.Where(item => item.IsDeleted))
            {
                // Add more granular check if items can be deleted independently of the parent
                item.IsDeleted = false;
            }


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                // Specific handling if restoring causes a unique constraint violation
                throw new ElementExistsException("RequestEntityID", id);
            }
            throw new StorageException(ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; } // Re-throw ElementNotFoundException
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    // Helper method to get an active request entity by ID, including related Customer (header only)
    // Use this when you only need the header data.
    private Task<Request?> GetRequestByID(string id)
    {
        return _dbContext.Requests
                         .Include(r => r.Customer) // Include Customer if always needed
                         .AsNoTracking() // Use AsNoTracking for read operations
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted); // Find by ID and exclude soft-deleted
    }
}