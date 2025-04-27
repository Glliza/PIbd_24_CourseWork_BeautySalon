using AutoMapper;
using BeautySalon.DataModels;
using BeautySalon.Entities;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Implementations;

internal class ReceiptSC : IReceiptSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public ReceiptSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Receipt, ReceiptDM>();
            cfg.CreateMap<ProductListItem, ProductListItemDM>();

            cfg.CreateMap<ReceiptDM, Receipt>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
               .ForMember(dest => dest.Products, opt => opt.Ignore()); // Handle list items manually

            cfg.CreateMap<ProductListItemDM, ProductListItem>()
               .ForMember(dest => dest.ParentReceiptID, opt => opt.Ignore()) // Parent FK handled manually
               .ForMember(dest => dest.ParentRequestID, opt => opt.Ignore()) // Parent FK handled manually
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()); // IsDeleted handled manually
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<ReceiptDM>> GetList(bool onlyActive = true, string? receiptID = null, string? staffID = null, string? customerID = null, bool? isCanceled = null, DateTime? fromDateIssued = null, DateTime? toDateIssued = null)
    {
        try
        {
            var query = _dbContext.Recepies // Use DbSet name
                                  .Include(r => r.Staff)
                                  .Include(r => r.Customer) // Customer is nullable
                                  .Include(r => r.CashBox)
                                  .Include(r => r.Visit) // Visit is nullable
                                  .AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (receiptID is not null) query = query.Where(x => x.ID == receiptID);
            if (staffID is not null) query = query.Where(x => x.StaffID == staffID);
            if (customerID is not null) query = query.Where(x => x.CustomerID == customerID);
            if (isCanceled is not null) query = query.Where(x => x.IsCanceled == isCanceled.Value);
            if (fromDateIssued is not null) query = query.Where(x => x.DateIssued >= fromDateIssued.Value);
            if (toDateIssued is not null) query = query.Where(x => x.DateIssued < toDateIssued.Value.AddDays(1));

            var receiptEntities = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ReceiptDM>>(receiptEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Receipt list: {ex.Message}", ex);
        }
    }

    public async Task<ReceiptDM?> GetElementByID(string id)
    {
        try
        {
            var receiptEntity = await GetReceiptByID(id);
            return _mapper.Map<ReceiptDM>(receiptEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Receipt by ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<ReceiptDM?> GetReceiptWithItemsAsync(string id)
    {
        try
        {
            var receiptEntity = await _dbContext.Recepies
                                                .Include(r => r.Staff)
                                                .Include(r => r.Customer)
                                                .Include(r => r.CashBox)
                                                .Include(r => r.Visit)
                                                .Include(r => r.Products) // Include ProductListItems
                                                    .ThenInclude(pli => pli.Product) // Include related Product
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);


            return _mapper.Map<ReceiptDM>(receiptEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Receipt with items by ID {id}: {ex.Message}", ex);
        }
    }

    public async Task AddElement(ReceiptDM receiptDataModel, string cashBoxId)
    {
        try
        {
            receiptDataModel.Validate(); // Includes validating list items

            var existingElement = await _dbContext.Recepies.AsNoTracking().FirstOrDefaultAsync(x => x.ID == receiptDataModel.ID);
            if (existingElement != null) throw new ElementExistsException("ID", receiptDataModel.ID);

            // Validate existence of referenced entities
            var staffExists = await _dbContext.Workers.AsNoTracking().AnyAsync(s => s.ID == receiptDataModel.StaffID && !s.IsDeleted);
            if (!staffExists) throw new ElementNotFoundException(receiptDataModel.StaffID, "Referenced Staff not found or is deleted.");

            if (!string.IsNullOrEmpty(receiptDataModel.CustomerID))
            {
                var customerExists = await _dbContext.Customers.AsNoTracking().AnyAsync(c => c.ID == receiptDataModel.CustomerID && !c.IsDeleted);
                if (!customerExists) throw new ElementNotFoundException(receiptDataModel.CustomerID ?? "null", "Referenced Customer not found or is deleted.");
            }

            var cashBoxExists = await _dbContext.CashBoxes.AsNoTracking().AnyAsync(cb => cb.ID == cashBoxId && !cb.IsDeleted); // Use passed cashBoxId
            if (!cashBoxExists) throw new ElementNotFoundException(cashBoxId, "Referenced CashBox not found or is deleted.");

            if (!string.IsNullOrEmpty(receiptDataModel.VisitID))
            {
                var visitExists = await _dbContext.Visits.AsNoTracking().AnyAsync(v => v.ID == receiptDataModel.VisitID && !v.IsDeleted);
                if (!visitExists) throw new ElementNotFoundException(receiptDataModel.VisitID ?? "null", "Referenced Visit not found or is deleted.");
            }

            // Validate existence of Products referenced by list items
            foreach (var productItemDm in receiptDataModel.Products)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID, $"Referenced Product '{productItemDm.ProductID}' in Product Items not found or is deleted.");
            }


            var receiptEntity = _mapper.Map<Receipt>(receiptDataModel);
            receiptEntity.IsDeleted = false;
            receiptEntity.CashBoxID = cashBoxId; // Set the CashBox FK from the parameter

            // Manually map and add list items, setting parent FK
            receiptEntity.Products = receiptDataModel.Products
                .Select(itemDm => {
                    var itemEntity = _mapper.Map<ProductListItem>(itemDm);
                    itemEntity.ParentReceiptID = receiptEntity.ID; // Set the FK back to the parent receipt
                                                                   // itemEntity.ParentRequestID = null; // Explicitly set other potential parent FKs to null
                    itemEntity.IsDeleted = false; // Items are not deleted initially
                    return itemEntity;
                }).ToList();


            await _dbContext.Recepies.AddAsync(receiptEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";
                if (sqlState == "23505") throw new ElementExistsException("Receipt", $"Adding failed due to unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
                if (sqlState == "23503") throw new StorageException($"Failed to add Receipt due to FK violation ('{constraintName}'). Ensure referenced entities exist. Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to add Receipt: {ex.Message}", ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"An unexpected error occurred while adding Receipt: {ex.Message}", ex); }
    }

    public async Task UpdElement(ReceiptDM receiptDataModel)
    {
        try
        {
            receiptDataModel.Validate();

            var element = await _dbContext.Recepies
                                          .Include(r => r.Products)
                                          .FirstOrDefaultAsync(x => x.ID == receiptDataModel.ID && !x.IsDeleted);

            if (element == null) throw new ElementNotFoundException(receiptDataModel.ID, "Active Receipt not found for update.");
            if (element.IsCanceled) throw new InvalidOperationException($"Cannot update a canceled Receipt {receiptDataModel.ID}."); // Business rule

            // Validate existence of updated related entities if IDs are changed (less common for Receipts)
            // ... checks for StaffID, CustomerID, VisitID if updatable ...

            // Validate existence of Products referenced by updated/new list items
            foreach (var productItemDm in receiptDataModel.Products)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID, $"Referenced Product '{productItemDm.ProductID}' in updated Product Items not found or is deleted.");
            }

            _mapper.Map(receiptDataModel, element); // Map header properties

            // Update Nested Collection (ProductItems)
            var currentProductItemIds = element.Products.Select(item => item.ID).ToList();
            var newItemProductItemIds = requestDataModel.ProductItems.Where(itemDm => !currentProductItemIds.Contains(itemDm.ID)).Select(itemDm => itemDm.ID).ToList(); // Use requestDataModel here for consistency? Assuming structure is the same
            var removedProductItems = element.Products.Where(itemEntity => !receiptDataModel.Products.Any(itemDm => itemDm.ID == itemEntity.ID)).ToList();

            foreach (var itemToRemove in removedProductItems) { _dbContext.ProductListItems.Remove(itemToRemove); /* Or Soft Delete */ }

            foreach (var newItemDm in receiptDataModel.Products.Where(itemDm => newItemProductItemIds.Contains(itemDm.ID))) // Use receiptDataModel
            {
                var newItemEntity = _mapper.Map<ProductListItem>(newItemDm);
                newItemEntity.ParentReceiptID = element.ID;
                // newItemEntity.ParentRequestID = null;
                newItemEntity.IsDeleted = false;
                element.Products.Add(newItemEntity);
            }


            var updatedProductItemIds = receiptDataModel.Products.Where(itemDm => currentProductItemIds.Contains(itemDm.ID)).Select(itemDm => itemDm.ID).ToList(); // Use receiptDataModel
            foreach (var updatedItemDm in receiptDataModel.Products.Where(itemDm => updatedProductItemIds.Contains(itemDm.ID))) // Use receiptDataModel
            {
                var existingItemEntity = element.Products.FirstOrDefault(item => item.ID == updatedItemDm.ID);
                if (existingItemEntity != null)
                {
                    _mapper.Map(updatedItemDm, existingItemEntity);
                    // existingItemEntity.IsDeleted = false; // if restoring via update
                }
            }


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";
                if (sqlState == "23505") throw new ElementExistsException("Receipt", $"Updating receipt {receiptDataModel.ID} failed due to unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
                if (sqlState == "23503") throw new StorageException($"Failed to update Receipt {receiptDataModel.ID} due to FK violation ('{constraintName}'). Ensure referenced entities exist. Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to update Receipt {receiptDataModel.ID}: {ex.Message}", ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (InvalidOperationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"An unexpected error occurred while updating Receipt {receiptDataModel.ID}: {ex.Message}", ex); }
    }

    public async Task DelElement(string id)
    {
        try
        {
            var element = await _dbContext.Recepies
                                          .Include(r => r.Products)
                                          .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);

            if (element == null) throw new ElementNotFoundException(id, "Active Receipt not found for deletion.");
            if (element.IsCanceled) throw new InvalidOperationException($"Cannot delete a canceled Receipt {id}."); // Business rule


            element.IsDeleted = true;

            // Cascading soft delete on items
            foreach (var item in element.Products) item.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"Failed to soft delete Receipt {id}: {ex.Message}", ex); }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (InvalidOperationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"An unexpected error occurred while soft deleting Receipt {id}: {ex.Message}", ex); }
    }

    public async Task RestoreElement(string id)
    {
        try
        {
            var element = await _dbContext.Recepies
                                          .Include(r => r.Products)
                                          .FirstOrDefaultAsync(x => x.ID == id && x.IsDeleted);

            if (element == null || !element.IsDeleted) throw new ElementNotFoundException(id, "No *deleted* Receipt found to restore.");

            // Optional: Check for conflicts upon restoration


            element.IsDeleted = false;

            _dbContext.Recepies.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;

            // Restore cascading soft-deleted items
            foreach (var item in element.Products.Where(item => item.IsDeleted)) item.IsDeleted = false;


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("Receipt", $"Restoring receipt {id} failed due to unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to restore Receipt {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"An unexpected error occurred while restoring Receipt {id}: {ex.Message}", ex); }
    }

    public async Task<ReceiptDM?> GetReceiptByVisitIdAsync(string visitId)
    {
        try
        {
            var receiptEntity = await _dbContext.Recepies
                                                .Include(r => r.Staff)
                                                .Include(r => r.Customer)
                                                .Include(r => r.CashBox)
                                                .Include(r => r.Visit)
                                                .Include(r => r.Products)
                                                    .ThenInclude(pli => pli.Product)
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.VisitID == visitId && !x.IsDeleted); // Find by VisitID

            return _mapper.Map<ReceiptDM>(receiptEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Receipt by Visit ID {visitId}: {ex.Message}", ex);
        }
    }


    // Helper method to get an active receipt entity by ID (header only)
    private Task<Receipt?> GetReceiptByID(string id)
    {
        return _dbContext.Recepies
                         .Include(r => r.Staff)
                         .Include(r => r.Customer)
                         .Include(r => r.CashBox)
                         .Include(r => r.Visit)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }

    // Helper method to get any receipt entity (including deleted) by ID (includes items for soft delete/restore logic)
    private Task<Receipt?> GetAnyReceiptByID(string id)
    {
        return _dbContext.Recepies
                        .Include(r => r.Products)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}
