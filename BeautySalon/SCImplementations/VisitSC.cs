using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using AutoMapper;

namespace BeautySalon.SCImplementations;

internal class VisitSC : IVisitSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public VisitSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;

        // Configure AutoMapper mapping profile for Visit
        var config = new MapperConfiguration(cfg =>
        {
            // Mapping from EF Entity to Core DM
            cfg.CreateMap<Visit, VisitDM>();

            // Mapping from Core DM to EF Entity
            // Ignore properties not present in DM (like IsDeleted) or handled separately
            cfg.CreateMap<VisitDM, Visit>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()); // IsDeleted is managed by SC logic, not mapped directly
        });
        _mapper = new Mapper(config); // Create Mapper instance
    }

    public async Task<List<VisitDM>> GetList(bool onlyActive = true, string? customerID = null, string? staffID = null, bool? status = null, DateTime? fromDateTimeOfVisit = null, DateTime? toDateTimeOfVisit = null)
    {
        try
        {
            var query = _dbContext.Visits
                                  .Include(v => v.Customer) // Include related Customer data
                                  .Include(v => v.Staff)    // Include related Staff data
                                                            // Decide if you need to include headers by default:
                                                            // .Include(v => v.ProductListHeader)
                                                            // .Include(v => v.ServiceListHeader)
                                  .AsQueryable();

            // Apply filters based on parameters
            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (customerID is not null)
            {
                query = query.Where(x => x.CustomerID == customerID);
            }
            if (staffID is not null)
            {
                query = query.Where(x => x.StaffID == staffID); // Use EF Entity property name (assuming StaffID FK name)
            }
            if (status is not null)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            if (fromDateTimeOfVisit is not null)
            {
                query = query.Where(x => x.DateTimeOfVisit >= fromDateTimeOfVisit.Value);
            }
            if (toDateTimeOfVisit is not null)
            {
                // Use AddDays(1) and < endDate for inclusive date range query common pattern
                query = query.Where(x => x.DateTimeOfVisit < toDateTimeOfVisit.Value.AddDays(1));
            }

            // Execute the query asynchronously and map results to DM
            var visitEntities = await query.AsNoTracking().ToListAsync(); // Use AsNoTracking for read-only queries
            return _mapper.Map<List<VisitDM>>(visitEntities); // Map List of Entities to List of DMs
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
    public async Task<VisitDM?> GetElementByID(string id)
    {
        try
        {
            var visitEntity = await GetVisitByID(id); // Use async helper method

            return _mapper.Map<VisitDM>(visitEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
    public async Task AddElement(VisitDM visitDataModel)
    {
        try
        {
            visitDataModel.Validate();


            // Check if an element with the same ID already exists (optional, but good practice for explicit ID assignment)
            var existingElement = await _dbContext.Visits.AsNoTracking().FirstOrDefaultAsync(x => x.ID == visitDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", visitDataModel.ID);
            }

            // Validate existence of related entities (Customer, Staff, ServiceListHeader, ProductListHeader if not null)
            // This is important to prevent FK constraint violations at the DB level.
            var customerExists = await _dbContext.Customers.AsNoTracking().AnyAsync(c => c.ID == visitDataModel.CustomerID && !c.IsDeleted);
            if (!customerExists) throw new ElementNotFoundException(visitDataModel.CustomerID);

            var staffExists = await _dbContext.Workers.AsNoTracking().AnyAsync(s => s.ID == visitDataModel.StaffID && !s.IsDeleted); // Use Workers DbSet name
            if (!staffExists) throw new ElementNotFoundException(visitDataModel.StaffID);

            var serviceListHeaderExists = await _dbContext.Services.AsNoTracking().AnyAsync(h => h.ID == visitDataModel.ServiceListID && !h.IsDeleted);
            if (!serviceListHeaderExists) throw new ElementNotFoundException(visitDataModel.ServiceListID);

            if (!string.IsNullOrEmpty(visitDataModel.ServiceListID))
            {
                var productListHeaderExists = await _dbContext.Products.AsNoTracking().AnyAsync(h => h.ID == visitDataModel.ServiceListID && !h.IsDeleted);
                if (!productListHeaderExists) throw new ElementNotFoundException(visitDataModel.ServiceListID);
            }

            var visitEntity = _mapper.Map<Visit>(visitDataModel);
            visitEntity.IsDeleted = false; // Explicitly set IsDeleted flag to false when adding

            // Add the entity to the DbContext change tracker
            await _dbContext.Visits.AddAsync(visitEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            // Check inner exception for specific database errors (e.g., unique index violation, FK violation)
            // For PostgreSQL Npgsql, unique constraint violation: SqlState '23505'
            // FK violation: SqlState '23503'
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";


                if (sqlState == "23505") // Unique constraint violation
                {
                    throw new ElementExistsException("VisitEntityName", constraintName);
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

    public async Task UpdElement(VisitDM visitDataModel)
    {
        try
        {
            visitDataModel.Validate();

            // Find the existing *active* entity by ID
            var element = await GetVisitByID(visitDataModel.ID);
            if (element == null)
            {
                throw new ElementNotFoundException(visitDataModel.ID);
            }

            // Prevent updating if soft-deleted
            if (element.IsDeleted)
            {
                throw new ElementNotFoundException(visitDataModel.ID);
            }

            // Validate existence of updated related entities if IDs are changed (less common, but robust)
            // If CustomerID, StaffID, ProductListID, or ServiceListID are allowed to change in DM:
            // if (element.CustomerID != visitDataModel.CustomerID) { /* check new CustomerID */ }
            // ... similar checks for other FKs ...

            // Map changes from DM to the existing EF Entity instance
            _mapper.Map(visitDataModel, element); // AutoMapper updates the existing 'element' entity
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null)
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Constraint";
                string sqlState = (ex.InnerException as Npgsql.PostgresException)?.SqlState ?? "N/A";


                if (sqlState == "23505") // Unique constraint violation (less likely on update unless updating unique index fields)
                {
                    throw new ElementExistsException("VisitEntityID", visitDataModel.ID);
                }
                else if (sqlState == "23503") // FK violation (e.g., changing an FK to a non-existent ID)
                {
                    throw new StorageException(ex);
                }
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
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
            // Find the active visit entity to soft delete
            var element = await GetVisitByID(id);
            if (element == null)
            {
                throw new ElementNotFoundException(id);
            }

            // Perform soft delete
            element.IsDeleted = true;

            // Note: You might also want to soft delete related entities here (e.g., the linked ProductListHeader and ServiceListHeader, or items within lists)
            // This depends on your business rules for cascading deletes/soft deletes.
            // If deleting headers/items, you'd load them here and set IsDeleted = true.
            // e.g., if (element.ProductListHeader != null) element.ProductListHeader.IsDeleted = true;
            // if (element.ServiceListHeader != null) element.ServiceListHeader.IsDeleted = true;
            // if (element.ServiceListHeader?.Items != null) foreach(var item in element.ServiceListHeader.Items) item.IsDeleted = true;
            // This requires including headers and items in the GetVisitByID helper method.

            // Save changes
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

    // Helper method to get an active visit entity by ID, including related Customer and Staff
    private Task<Visit?> GetVisitByID(string id)
    {
        return _dbContext.Visits
                         .Include(v => v.Customer)
                         .Include(v => v.Staff)
                         .Include(v => v.Request) // to link the request
                         .Include(v => v.Services)
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }
}