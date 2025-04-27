using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using BeautySalon.Enums;


namespace BeautySalon.Implementations;
// ReceiptSC, ProductSC, ServiceSC, ShiftSC, CashBoxSC

internal class StaffSC : IStaffSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    // Constructor injects the DbContext
    public StaffSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;

        // Configure AutoMapper mapping profile
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Staff, StaffDM>().ReverseMap(); // Two-way mapping
            // Ensure enum mapping is handled if storing enums as strings or ints
            // cfg.CreateMap<PostType, string>().ConvertUsing(e => e.ToString());
            // cfg.CreateMap<string, PostType>().ConvertUsing(s => Enum.Parse<PostType>(s));
        });
        _mapper = new Mapper(config);
    }

    // Implement methods: (async)

    public async Task<List<StaffDM>> GetList(bool onlyActive = true, string? staffID = null,
        DateTime? fromBirthDate = null, DateTime? toBirthDate = null,
        DateTime? fromEmploymentDate = null, DateTime? toEmploymentDate = null,
        PostType? postType = null)
    {
        try
        {
            var query = _dbContext.Workers.AsQueryable();

            // * filters based on parameters
            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (staffID is not null)
            {
                query = query.Where(x => x.ID == staffID);
            }
            if (fromBirthDate is not null)
            {
                query = query.Where(x => x.BirthDate >= fromBirthDate.Value);
            }
            if (toBirthDate is not null)
            {
                query = query.Where(x => x.BirthDate < toBirthDate.Value.AddDays(1));
            }
            if (fromEmploymentDate is not null)
            {
                query = query.Where(x => x.EmploymentDate >= fromEmploymentDate.Value);
            }
            if (toEmploymentDate is not null)
            {
                query = query.Where(x => x.EmploymentDate < toEmploymentDate.Value.AddDays(1));
            }
            if (postType is not null)
            {
                query = query.Where(x => x.postType == postType.Value);
            }

            // Execute the query asynchronously and map results to DM
            var staffEntities = await query.ToListAsync();
            return _mapper.Map<List<StaffDM>>(staffEntities);
            // Map List of Entities to List of DMs
        }
        catch (Exception ex)
        {
            // Clear the change tracker on error to prevent inconsistent state
            _dbContext.ChangeTracker.Clear();
            // Wrap the exception in a custom StorageException
            throw new StorageException($"Failed to get Staff list: {ex.Message}", ex);
        }
    }

    public async Task<StaffDM?> GetElementByID(string id)
    {
        try
        {
            var staffEntity = await GetStaffByID(id);

            // Map the entity to DM, returns null if entity is null
            return _mapper.Map<StaffDM>(staffEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Staff by ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<StaffDM?> GetElementByFIO(string fio)
    {
        try
        {
            var staffEntity = await _dbContext.Workers
                                               .AsNoTracking() // Use AsNoTracking for read-only queries
                                               .FirstOrDefaultAsync(x => x.FIO == fio && !x.IsDeleted); // Use async method
            return _mapper.Map<StaffDM>(staffEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Staff by FIO '{fio}': {ex.Message}", ex);
        }
    }

    public async Task AddElement(StaffDM staffDataModel)
    {
        try
        {
            // Validate the incoming data model
            staffDataModel.Validate();

            // Check if an element with the same ID already exists (optional, but good practice for explicit ID assignment)
            var existingElement = await _dbContext.Workers.AsNoTracking().FirstOrDefaultAsync(x => x.ID == staffDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", staffDataModel.ID);
            }

            // Map DM to Entity
            var staffEntity = _mapper.Map<Staff>(staffDataModel);

            // Add the entity to the DbContext change tracker
            await _dbContext.Workers.AddAsync(staffEntity); // Use async Add

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }
        // Catch specific exceptions for better handling if needed (e.g., unique constraints)
        catch (DbUpdateException ex) // Catch EF Core specific exceptions
        {
            _dbContext.ChangeTracker.Clear();
            // Check inner exception for specific database errors (e.g., unique index violation)
            // This requires knowledge of the database provider's exception types (e.g., PostgresException)
            // For PostgreSQL Npgsql, unique constraint violation often has SqlState '23505'
            if (ex.InnerException?.Message?.Contains("unique constraint") ?? false || (ex.InnerException?.GetType().Name == "PostgresException" && ex.InnerException.Message.Contains("23505")))
            {
                throw new ElementExistsException("Staff", $"ID/FIO combination or other unique field value already exists. Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to add Staff: {ex.Message}", ex); // Re-throw as generic StorageException
        }
        catch (ValidationException)
        {
            _dbContext.ChangeTracker.Clear();
            throw; // Re-throw validation exceptions directly
        }
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while adding Staff: {ex.Message}", ex);
        }
    }

    public async Task UpdElement(StaffDM staffDataModel)
    {
        try
        {
            // Validate the incoming data model
            staffDataModel.Validate();

            // Find the existing entity by ID, *including* soft-deleted ones if needed for update/restore flow,
            // but typically you only update active ones. Let's find the active one.
            var element = await GetStaffByID(staffDataModel.ID); // Use async helper method
            if (element == null)
            {
                throw new ElementNotFoundException(staffDataModel.ID);
            }

            // Ensure you're not trying to update a soft-deleted element if onlyActive logic is standard
            if (element.IsDeleted)
            {
                throw new ElementNotFoundException(staffDataModel.ID, "Cannot update a deleted Staff.");
            }

            // Map changes from DM to the existing EF Entity instance
            _mapper.Map(staffDataModel, element);

            // Mark the entity as Modified (EF Core might track this automatically if loaded)
            // _dbContext.Entry(element).State = EntityState.Modified; // Optional, often not needed after loading

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }
        // Catch specific exceptions for better handling
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            // Check inner exception for unique constraint violations on updated fields (e.g., FIO if it was updated)
            if (ex.InnerException?.Message?.Contains("unique constraint") ?? false || (ex.InnerException?.GetType().Name == "PostgresException" && ex.InnerException.Message.Contains("23505")))
            {
                throw new ElementExistsException("Staff", $"Updated FIO or other unique field value already exists. Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to update Staff {staffDataModel.ID}: {ex.Message}", ex);
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
            throw new StorageException($"An unexpected error occurred while updating Staff {staffDataModel.ID}: {ex.Message}", ex);
        }
    }

    public async Task DelElement(string id)
    {
        try
        {
            // Find the active staff entity to delete
            var element = await GetStaffByID(id); // Use async helper method to find active
            if (element == null)
            {
                throw new ElementNotFoundException(id);
            }

            element.IsDeleted = true;

            // Save changes
            await _dbContext.SaveChangesAsync();
        }

        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to soft delete Staff {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) // Catch your custom not found exception
        {
            _dbContext.ChangeTracker.Clear();
            throw; // Re-throw ElementNotFoundException directly
        }
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while soft deleting Staff {id}: {ex.Message}", ex);
        }
    }

    public async Task RestoreElement(string id)
    {
        try
        {
            // Find the soft-deleted staff entity to restore
            // Note: Need to find *including* deleted ones here
            var element = await _dbContext.Workers
                                          .AsNoTracking() // Optional, could track if you re-attach
                                          .FirstOrDefaultAsync(x => x.ID == id && x.IsDeleted); // Find only deleted

            if (element == null)
            {
                // Can throw ElementNotFoundException or a more specific one if needed
                throw new ElementNotFoundException(id, "No deleted Staff found with this ID to restore.");
            }

            // Restore the element
            element.IsDeleted = false;

            // Attach the entity if AsNoTracking was used, and mark as modified
            _dbContext.Workers.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;


            // Save changes
            await _dbContext.SaveChangesAsync();
        }
        // Catch specific exceptions for better handling
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            // Could potentially have unique constraint violation if restoring would create duplicates (e.g. FIO)
            // Add specific handling if necessary.
            throw new StorageException($"Failed to restore Staff {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) // Catch your custom not found exception
        {
            _dbContext.ChangeTracker.Clear();
            throw; // Re-throw ElementNotFoundException directly
        }
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while restoring Staff {id}: {ex.Message}", ex);
        }
    }

    // Helper method to get an active staff entity by ID
    private Task<Staff?> GetStaffByID(string id)
    {
        return _dbContext.Workers
                         .AsNoTracking() // Use AsNoTracking for read operations
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted); // Find by ID and exclude soft-deleted
    }

    // Helper method to get any staff entity (including deleted) by ID
    private Task<Staff?> GetAnyStaffByID(string id)
    {
        return _dbContext.Workers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}