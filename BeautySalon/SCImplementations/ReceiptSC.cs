using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using AutoMapper;

namespace BeautySalon.SCImplementations;

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
               .ForMember(dest => dest.Products, opt => opt.Ignore()); // Handle list items manually

            cfg.CreateMap<ProductListItemDM, ProductListItem>()
               .ForMember(dest => dest.ParentReceiptID, opt => opt.Ignore()) // FK handled manually
               .ForMember(dest => dest.ParentRequestID, opt => opt.Ignore()) // ^^^
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<ReceiptDM>> GetList(bool onlyActive = true, string? staffID = null, string? customerID = null, bool? isCanceled = null, DateTime? fromDateIssued = null, DateTime? toDateIssued = null)
    {
        try
        {
            var query = _dbContext.Receipts // Use DbSet name
                                  .Include(r => r.Staff)
                                  .Include(r => r.Customer) // Customer is nullable
                                  .Include(r => r.CashBox)
                                  .AsQueryable();

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
            throw new StorageException(ex);
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
            throw new StorageException(ex);
        }
    }

    public async Task<ReceiptDM?> GetReceiptWithItemsAsync(string id)
    {
        try
        {
            var receiptEntity = await _dbContext.Receipts
                                                .Include(r => r.Staff)
                                                .Include(r => r.Customer)
                                                .Include(r => r.CashBox)
                                                .Include(r => r.Products) // Include ProductListItems
                                                    .ThenInclude(pli => pli.Product) // Include related Product
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.ID == id);


            return _mapper.Map<ReceiptDM>(receiptEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task AddElement(ReceiptDM receiptDataModel, string cashBoxId)
    {
        try
        {
            receiptDataModel.Validate(); // Includes validating list items

            var existingElement = await _dbContext.Receipts.AsNoTracking().FirstOrDefaultAsync(x => x.ID == receiptDataModel.ID);
            if (existingElement != null) throw new ElementExistsException("ID", receiptDataModel.ID);

            // Validate existence of referenced entities
            var staffExists = await _dbContext.Workers.AsNoTracking().AnyAsync(s => s.ID == receiptDataModel.StaffID && !s.IsDeleted);
            if (!staffExists) throw new ElementNotFoundException(receiptDataModel.StaffID);

            if (!string.IsNullOrEmpty(receiptDataModel.CustomerID))
            {
                var customerExists = await _dbContext.Customers.AsNoTracking().AnyAsync(c => c.ID == receiptDataModel.CustomerID && !c.IsDeleted);
                if (!customerExists) throw new ElementNotFoundException(receiptDataModel.CustomerID ?? "null");
            }

            var cashBoxExists = await _dbContext.CashBoxes.AsNoTracking().AnyAsync(cb => cb.ID == cashBoxId && !cb.IsDeleted); // Use passed cashBoxId
            if (!cashBoxExists) throw new ElementNotFoundException(cashBoxId);

            // if (!string.IsNullOrEmpty(receiptDataModel.VisitID)) var visitExists = await _dbContext.Visits.AsNoTracking().AnyAsync(v => v.ID == receiptDataModel.VisitID && !v.IsDeleted); ...

            // Validate existence of Products referenced by list items
            foreach (var productItemDm in receiptDataModel.Products)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID);
            }

            var receiptEntity = _mapper.Map<Receipt>(receiptDataModel);
            receiptEntity.CashBoxID = cashBoxId; // Set the CashBox FK from the parameter

            // Manually map and add list items, setting parent FK
            receiptEntity.Products = receiptDataModel.Products
                .Select(itemDm => {
                    var itemEntity = _mapper.Map<ProductListItem>(itemDm);
                    itemEntity.ParentReceiptID = receiptEntity.ID; // Set the FK back to the parent receipt
                                                                   // itemEntity.ParentRequestID = null;
                                                                   // Explicitly set other potential parent FKs to null
                    itemEntity.IsDeleted = false; // > not deleted initially
                    return itemEntity;
                }).ToList();


            await _dbContext.Receipts.AddAsync(receiptEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";
                if (sqlState == "23505") throw new ElementExistsException("ReceiptConstraintName", constraintName);
                if (sqlState == "23503") throw new StorageException(ex);
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task UpdElement(ReceiptDM receiptDataModel)
    {
        try
        {
            receiptDataModel.Validate();

            var element = await _dbContext.Receipts
                                          .Include(r => r.Products)
                                          .FirstOrDefaultAsync(x => x.ID == receiptDataModel.ID);

            if (element == null) throw new ElementNotFoundException(receiptDataModel.ID);
            if (element.IsCanceled) throw new InvalidOperationException($"Cannot update a canceled Receipt {receiptDataModel.ID}."); // Business rule

            // Validate existence of updated related entities if IDs are changed (less common for Receipts)
            // ... checks for StaffID, CustomerID if updatable ...

            // Validate existence of Products referenced by updated/new list items
            foreach (var productItemDm in receiptDataModel.Products)
            {
                var productExists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.ID == productItemDm.ProductID && !p.IsDeleted);
                if (!productExists) throw new ElementNotFoundException(productItemDm.ProductID);
            }

            _mapper.Map(receiptDataModel, element); // Map header properties

            // Update Nested Collection (ProductItems)
            var currentProductItemIds = element.Products.Select(item => item.ID).ToList();
            var newItemProductItemIds = receiptDataModel.Products.Where(itemDm => !currentProductItemIds.Contains(itemDm.ProductID)).Select(itemDm => itemDm.ProductID).ToList(); // Use requestDataModel here for consistency? Assuming structure is the same
            var removedProductItems = element.Products.Where(itemEntity => !receiptDataModel.Products.Any(itemDm => itemDm.ProductID == itemEntity.ID)).ToList();

            foreach (var itemToRemove in removedProductItems) { _dbContext.ProductListItems.Remove(itemToRemove); /* Or Soft Delete */ }

            foreach (var newItemDm in receiptDataModel.Products.Where(itemDm => newItemProductItemIds.Contains(itemDm.ProductID))) // Use receiptDataModel
            {
                var newItemEntity = _mapper.Map<ProductListItem>(newItemDm);
                newItemEntity.ParentReceiptID = element.ID;
                newItemEntity.ParentRequestID = null;
                newItemEntity.IsDeleted = false;
                element.Products.Add(newItemEntity);
            }


            var updatedProductItemIds = receiptDataModel.Products.Where(itemDm => currentProductItemIds.Contains(itemDm.ProductID)).Select(itemDm => itemDm.ProductID).ToList(); // Use receiptDataModel
            foreach (var updatedItemDm in receiptDataModel.Products.Where(itemDm => updatedProductItemIds.Contains(itemDm.ProductID))) // Use receiptDataModel
            {
                var existingItemEntity = element.Products.FirstOrDefault(item => item.ID == updatedItemDm.ProductID);
                if (existingItemEntity != null)
                {
                    _mapper.Map(updatedItemDm, existingItemEntity);
                    // existingItemEntity.IsDeleted = false; --> if restoring via update
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
                if (sqlState == "23505") throw new ElementExistsException("ReceiptEntityID", receiptDataModel.ID);
                if (sqlState == "23503") throw new StorageException(ex);
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (InvalidOperationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task DelElement(string id)
    {
        try
        {
            var element = await _dbContext.Receipts
                                          .Include(r => r.Products)
                                          .FirstOrDefaultAsync(x => x.ID == id);

            if (element == null) throw new ElementNotFoundException(id);
            if (element.IsCanceled) throw new InvalidOperationException($"Cannot delete a canceled Receipt {id}."); // Business rule

            // Cascading soft delete on items
            foreach (var item in element.Products) item.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (InvalidOperationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    // Get an active receipt entity by ID (header only):
    private Task<Receipt?> GetReceiptByID(string id)
    {
        return _dbContext.Receipts
                         .Include(r => r.Staff)
                         .Include(r => r.Customer)
                         .Include(r => r.CashBox)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id);
    }
}
