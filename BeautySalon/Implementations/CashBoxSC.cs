using AutoMapper;
using BeautySalon.DataModels;
using BeautySalon.Entities;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Implementations;

internal class CashBoxSC : ICashBoxSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper; // Using concrete Mapper as in your template

    public CashBoxSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CashBox, CashBoxDM>();
            // Ignore properties not present in DM (like IsDeleted) or handled separately
            cfg.CreateMap<CashBoxDM, CashBox>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()); // IsDeleted is managed by SC logic, not mapped directly
        });
        _mapper = new Mapper(config); // Create Mapper instance
    }

    public async Task<List<CashBoxDM>> GetList(bool onlyActive = true)
    {
        try
        {
            var query = _dbContext.CashBoxes.AsQueryable(); // Use the correct DbSet name
            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            // Execute the query asynchronously and map results to DM
            var cashBoxEntities = await query.AsNoTracking().ToListAsync(); // Use AsNoTracking for read-only queries
            return _mapper.Map<List<CashBoxDM>>(cashBoxEntities); // Map List of Entities to List of DMs
        }
        catch (Exception ex)
        {
            // Clear the change tracker on error to prevent inconsistent state
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get CashBox list: {ex.Message}", ex);
        }
    }

    public async Task<CashBoxDM?> GetElementByID(string id)
    {
        try
        {
            // Find the active cash box entity by ID
            var cashBoxEntity = await GetCashBoxByID(id); // Use async helper method

            // Map the entity to DM, returns null if entity is null
            return _mapper.Map<CashBoxDM>(cashBoxEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get CashBox by ID {id}: {ex.Message}", ex);
        }
    }

    public async Task AddElement(CashBoxDM cashBoxDataModel)
    {
        try
        {
            cashBoxDataModel.Validate();

            var existingElement = await _dbContext.CashBoxes.AsNoTracking().FirstOrDefaultAsync(x => x.ID == cashBoxDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", cashBoxDataModel.ID);
            }

            var cashBoxEntity = _mapper.Map<CashBox>(cashBoxDataModel);
            cashBoxEntity.IsDeleted = false; // Explicitly set IsDeleted flag to false when adding

            // Add the entity to the DbContext change tracker
            await _dbContext.CashBoxes.AddAsync(cashBoxEntity); // Use async Add


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) // Catch EF Core specific exceptions
        {
            _dbContext.ChangeTracker.Clear();
            // Check inner exception for specific database errors (e.g., unique index violation)
            // For PostgreSQL Npgsql, unique constraint violation often has SqlState '23505'
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                // Attempt to provide more detail based on the constraint name if available
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("CashBox", $"Adding failed due to a unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
            }
            // If not a unique constraint violation, re-throw as a generic storage exception
            throw new StorageException($"Failed to add CashBox: {ex.Message}", ex);
        }
        catch (ValidationException)
        {
            _dbContext.ChangeTracker.Clear();
            throw; // Re-throw validation exceptions directly
        }
        catch (ElementExistsException)
        {
            _dbContext.ChangeTracker.Clear();
            throw; // Re-throw ElementExistsException directly
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while adding CashBox: {ex.Message}", ex);
        }
    }

    public async Task UpdElement(CashBoxDM cashBoxDataModel)
    {
        try
        {
            cashBoxDataModel.Validate();

            // Find the existing *active* entity by ID
            var element = await GetCashBoxByID(cashBoxDataModel.ID); // Use async helper method to find active
            if (element == null)
            {
                // If GetCashBoxByID (which filters !IsDeleted) returns null, the element is not found or is deleted
                throw new ElementNotFoundException(cashBoxDataModel.ID, "Active CashBox not found with this ID for update.");
            }

            if (element.IsDeleted)
            {
                throw new ElementNotFoundException(cashBoxDataModel.ID, "Cannot update a deleted CashBox.");
            }

            _mapper.Map(cashBoxDataModel, element);
            // EF Core often tracks changes automatically if 'element' was loaded in the current context
            // If using AsNoTracking and re-attaching, you'd need: _dbContext.Entry(element).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            // Unique constraint violations on update are less likely for CashBox unless updating a unique field other than ID
            throw new StorageException($"Failed to update CashBox {cashBoxDataModel.ID}: {ex.Message}", ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException($"An unexpected error occurred while updating CashBox {cashBoxDataModel.ID}: {ex.Message}", ex); }}

    public async Task DelElement(string id)
    {
        try
        {
            // Find the active cash box entity to soft delete
            var element = await GetCashBoxByID(id); // Use async helper method to find active
            if (element == null)
            {
                throw new ElementNotFoundException(id, "Active CashBox not found with this ID for deletion.");
            }

            // Perform soft delete
            element.IsDeleted = true;


            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to soft delete CashBox {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while soft deleting CashBox {id}: {ex.Message}", ex);
        }
    }

    public async Task RestoreElement(string id)
    {
        try
        {
            // Find the soft-deleted cash box entity to restore
            // Note: Need to find *including* deleted ones here and check if it *is* deleted
            var element = await GetAnyCashBoxByID(id);

            if (element == null || !element.IsDeleted) // Check if found AND is currently deleted
            {
                throw new ElementNotFoundException(id, "No *deleted* CashBox found with this ID to restore.");
            }

            // Restore the element
            element.IsDeleted = false;

            // Attach the entity if AsNoTracking was used to load it, and mark as modified
            _dbContext.CashBoxes.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                // Specific handling if restoring causes a unique constraint violation
                throw new ElementExistsException("CashBox", $"Restoring cash box {id} failed due to a unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to restore CashBox {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while restoring CashBox {id}: {ex.Message}", ex);
        }
    }

    // Helper method to get an active cash box entity by ID
    private Task<CashBox?> GetCashBoxByID(string id)
    {
        return _dbContext.CashBoxes
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted); // Find by ID and exclude soft-deleted
    }

    // Helper method to get any cash box entity (including deleted) by ID
    private Task<CashBox?> GetAnyCashBoxByID(string id)
    {
        return _dbContext.CashBoxes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}