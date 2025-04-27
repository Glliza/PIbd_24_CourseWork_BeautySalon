using AutoMapper;
using BeautySalon.DataModels;
using BeautySalon.Entities;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Implementations;

internal class ShiftSC : IShiftSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public ShiftSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Shift, ShiftDM>();

            // Mapping from Core DM to EF Entity
            // Ignore properties not present in DM (like IsDeleted) or handled separately
            cfg.CreateMap<ShiftDM, Shift>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            // IsDeleted is managed by SC logic, not mapped directly
            // Ignore navigation properties on the EF entity if you don't map them directly
            // .ForMember(dest => dest.CashBox, opt => opt.Ignore())
            // .ForMember(dest => dest.Staff, opt => opt.Ignore());
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<ShiftDM>> GetList(bool onlyActive = true, string? shiftID = null, string? staffID = null, string? cashBoxID = null, DateTime? fromDateTimeStart = null, DateTime? toDateTimeStart = null, DateTime? fromDateTimeFinish = null, DateTime? toDateTimeFinish = null)
    {
        try
        {
            var query = _dbContext.Shifts
                                  .Include(s => s.CashBox) // Include related CashBox data
                                  .Include(s => s.Staff)   // Include related Staff data
                                  .AsQueryable();
            if (onlyActive)
            {
                // "Active" typically means not soft-deleted AND DateTimeFinish is null
                query = query.Where(x => !x.IsDeleted && x.DateTimeFinish == null);
            }
            else
            {
                // If not 'onlyActive', filter by IsDeleted flag explicitly if needed,
                // or just include all regardless of IsDeleted state if onlyActive=false means *all* shifts.
                // Assuming onlyActive=false means include soft-deleted ones in the results.
                query = query.Where(x => !x.IsDeleted); // Keep only active shifts + soft-deleted active=false
                                                        // If you truly need *all* shifts including hard-deleted (not our pattern), remove this line.
            }

            if (shiftID is not null)
            {
                query = query.Where(x => x.ID == shiftID);
            }
            if (staffID is not null)
            {
                query = query.Where(x => x.StaffID == staffID);
            }
            if (cashBoxID is not null)
            {
                query = query.Where(x => x.CashBoxID == cashBoxID);
            }
            if (fromDateTimeStart is not null)
            {
                query = query.Where(x => x.DateTimeStart >= fromDateTimeStart.Value);
            }
            if (toDateTimeStart is not null)
            {
                // Use AddDays(1) and < endDate for inclusive date range query common pattern for dates
                // For DateTime, exact comparison or range >= start && < end+1day is needed
                query = query.Where(x => x.DateTimeStart <= toDateTimeStart.Value); // Assuming inclusivity
            }
            // Filter by DateTimeFinish range (only if DateTimeFinish is not null)
            if (fromDateTimeFinish is not null)
            {
                query = query.Where(x => x.DateTimeFinish != null && x.DateTimeFinish.Value >= fromDateTimeFinish.Value);
            }
            if (toDateTimeFinish is not null)
            {
                query = query.Where(x => x.DateTimeFinish != null && x.DateTimeFinish.Value <= toDateTimeFinish.Value); // Assuming inclusivity
            }

            var shiftEntities = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ShiftDM>>(shiftEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Shift list: {ex.Message}", ex);
        }
    }


    public async Task<ShiftDM?> GetElementByID(string id)
    {
        try
        {
            var shiftEntity = await GetShiftByID(id);
            return _mapper.Map<ShiftDM>(shiftEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get Shift by ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<ShiftDM?> GetActiveShiftForStaffAsync(string staffID)
    {
        try
        {
            var shiftEntity = await _dbContext.Shifts
                                               .Include(s => s.CashBox) // Include related CashBox
                                               .Include(s => s.Staff)    // Include related Staff
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync(x => x.StaffID == staffID && !x.IsDeleted && x.DateTimeFinish == null);

            return _mapper.Map<ShiftDM>(shiftEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to get active Shift for Staff ID {staffID}: {ex.Message}", ex);
        }
    }

    public async Task AddElement(ShiftDM shiftDataModel)
    {
        try
        {
            shiftDataModel.Validate();

            var existingElement = await _dbContext.Shifts.AsNoTracking().FirstOrDefaultAsync(x => x.ID == shiftDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", shiftDataModel.ID);
            }

            // Check if the referenced CashBox and Staff exist and are not deleted
            var cashBoxExists = await _dbContext.CashBoxes.AsNoTracking().AnyAsync(cb => cb.ID == shiftDataModel.CashBoxID && !cb.IsDeleted);
            if (!cashBoxExists) throw new ElementNotFoundException(shiftDataModel.CashBoxID, "Referenced CashBox not found or is deleted.");

            var staffExists = await _dbContext.Workers.AsNoTracking().AnyAsync(s => s.ID == shiftDataModel.StaffID && !s.IsDeleted); // Use Workers DbSet name
            if (!staffExists) throw new ElementNotFoundException(shiftDataModel.StaffID, "Referenced Staff not found or is deleted.");

            // Check if the staff member already has an active shift
            var activeShift = await GetActiveShiftForStaffAsync(shiftDataModel.StaffID);
            if (activeShift != null)
            {
                throw new ElementExistsException("StaffID", shiftDataModel.StaffID, $"Staff member {shiftDataModel.StaffID} already has an active shift (ID: {activeShift.ID}).");
            }

            // Map DM to EF Entity
            var shiftEntity = _mapper.Map<Shift>(shiftDataModel);
            shiftEntity.IsDeleted = false;

            await _dbContext.Shifts.AddAsync(shiftEntity);

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
                    throw new ElementExistsException("Shift", $"Adding failed due to a unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
                }
                else if (sqlState == "23503") // FK violation
                {
                    throw new StorageException($"Failed to add Shift due to a Foreign Key constraint violation ('{constraintName}'). Ensure referenced CashBox and Staff exist. Details: {ex.InnerException.Message}", ex);
                }
            }
            throw new StorageException($"Failed to add Shift: {ex.Message}", ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while adding Shift: {ex.Message}", ex);
        }
    }

    public async Task UpdElement(ShiftDM shiftDataModel)
    {
        try
        {
            shiftDataModel.Validate();

            var element = await GetShiftByID(shiftDataModel.ID);
            if (element == null)
            {
                // If GetShiftByID (which filters !IsDeleted and DateTimeFinish == null) returns null, the element is not found or is already finished/deleted
                throw new ElementNotFoundException(shiftDataModel.ID, "Active Shift not found with this ID for update.");
            }

            // If the update includes DateTimeFinish, ensure it's after DateTimeStart (validation in DM should cover this, but double-check)
            if (shiftDataModel.DateTimeFinish != null && shiftDataModel.DateTimeFinish.Value <= element.DateTimeStart)
            {
                throw new ValidationException($"Shift {shiftDataModel.ID}: DateTimeFinish must be after DateTimeStart.");
            }

            // Validate existence of updated related entities if IDs are changed (less common for Shift)
            // If CashBoxID or StaffID are allowed to change in DM:
            // if (element.CashBoxID != shiftDataModel.CashBoxID) { /* check new CashBoxID */ }
            // if (element.StaffID != shiftDataModel.StaffID) { /* check new StaffID */ }

            _mapper.Map(shiftDataModel, element);

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
                    throw new ElementExistsException("Shift", $"Updating shift {shiftDataModel.ID} failed due to a unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
                }
                else if (sqlState == "23503") // FK violation (e.g., changing an FK to a non-existent ID)
                {
                    throw new StorageException($"Failed to update Shift {shiftDataModel.ID} due to a Foreign Key constraint violation ('{constraintName}'). Ensure referenced entities exist. Details: {ex.InnerException.Message}", ex);
                }
            }
            throw new StorageException($"Failed to update Shift {shiftDataModel.ID}: {ex.Message}", ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while updating Shift {shiftDataModel.ID}: {ex.Message}", ex);
        }
    }

    public async Task DelElement(string id)
    {
        try
        {
            var element = await GetShiftByID(id);
            if (element == null)
            {
                throw new ElementNotFoundException(id, "Active Shift not found with this ID for deletion.");
            }

            // Prevent deleting a shift that hasn't finished yet (business rule)
            if (element.DateTimeFinish == null)
            {
                throw new InvalidOperationException($"Shift {id} cannot be deleted because it has not finished yet.");
            }

            // Perform soft delete
            element.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to soft delete Shift {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (InvalidOperationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while soft deleting Shift {id}: {ex.Message}", ex);
        }
    }

    // Implement RestoreElement method (async)
    public async Task RestoreElement(string id)
    {
        try
        {
            // Find the soft-deleted shift entity to restore
            // Note: Need to find *including* deleted ones here and check if it *is* deleted
            var element = await GetAnyShiftByID(id); // Use async helper method to find any

            if (element == null || !element.IsDeleted) // Check if found AND is currently deleted
            {
                throw new ElementNotFoundException(id, "No *deleted* Shift found with this ID to restore.");
            }

            // Restore the element
            element.IsDeleted = false;

            // Attach the entity if AsNoTracking was used to load it, and mark as modified
            _dbContext.Shifts.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;


            // Save changes
            await _dbContext.SaveChangesAsync();
        }
        // Catch specific exceptions for better handling
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                // Specific handling if restoring causes a unique constraint violation
                throw new ElementExistsException("Shift", $"Restoring shift {id} failed due to a unique constraint violation ('{constraintName}'). Details: {ex.InnerException.Message}", ex);
            }
            throw new StorageException($"Failed to restore Shift {id}: {ex.Message}", ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; } // Re-throw ElementNotFoundException
        catch (Exception ex) // Catch any other unexpected exceptions
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"An unexpected error occurred while restoring Shift {id}: {ex.Message}", ex);
        }
    }

    public async Task EndShiftAsync(string shiftId, DateTime dateTimeFinish)
    {
        try
        {
            // Find the active shift by ID for tracking
            var element = await _dbContext.Shifts.FirstOrDefaultAsync(x => x.ID == shiftId && !x.IsDeleted && x.DateTimeFinish == null);

            if (element == null)
            {
                throw new ElementNotFoundException(shiftId, "Active Shift not found with this ID to end.");
            }

            // Validate that the finish time is after the start time
            if (dateTimeFinish <= element.DateTimeStart)
            {
                _dbContext.ChangeTracker.Clear(); // Clear changes before throwing
                throw new ValidationException($"Finish time ({dateTimeFinish}) must be after the shift start time ({element.DateTimeStart}).");
            }

            // Set the finish time
            element.DateTimeFinish = dateTimeFinish;

            // Save changes
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException($"Failed to end Shift {shiftId}: {ex.Message}", ex);
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
            throw new StorageException($"An unexpected error occurred while ending Shift {shiftId}: {ex.Message}", ex);
        }
    }


    // Helper method to get an active shift entity by ID
    private Task<Shift?> GetShiftByID(string id)
    {
        // An active shift is not soft-deleted AND has no finish time
        return _dbContext.Shifts
                         .Include(s => s.CashBox) // Include related CashBox
                         .Include(s => s.Staff)    // Include related Staff
                         .AsNoTracking() // Use AsNoTracking for read operations
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted && x.DateTimeFinish == null);
    }

    // Helper method to get any shift entity (including deleted and finished) by ID
    private Task<Shift?> GetAnyShiftByID(string id)
    {
        return _dbContext.Shifts
                        .Include(s => s.CashBox) // Include related CashBox
                        .Include(s => s.Staff)    // Include related Staff
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}