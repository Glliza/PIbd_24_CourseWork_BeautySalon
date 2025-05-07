using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using AutoMapper;

namespace BeautySalon.SCImplementations;

internal class CustomerSC : ICustomerSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public CustomerSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Customer, CustomerDM>().ReverseMap();
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<CustomerDM>> GetList(bool onlyActive = true, string? fio = null, string? phoneNumber = null, DateTime? fromBirthDate = null, DateTime? toBirthDate = null)
    {
        try
        {
            var query = _dbContext.Customers.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (fio is not null)
            {
                query = query.Where(x => x.FIO.Contains(fio)); // Use Contains for partial FIO search
            }
            if (phoneNumber is not null)
            {
                query = query.Where(x => x.PhoneNumber == phoneNumber); // Exact match for phone number
            }
            if (fromBirthDate is not null)
            {
                query = query.Where(x => x.BirthDate >= fromBirthDate.Value);
            }
            if (toBirthDate is not null)
            {
                query = query.Where(x => x.BirthDate < toBirthDate.Value.AddDays(1));
            }

            var customerEntities = await query.AsNoTracking().ToListAsync(); // Use AsNoTracking
            return _mapper.Map<List<CustomerDM>>(customerEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<CustomerDM?> GetElementByID(string id)
    {
        try
        {
            var customerEntity = await GetCustomerByID(id);
            return _mapper.Map<CustomerDM>(customerEntity); // [ !!! ] *
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<CustomerDM?> GetElementByPhoneNumber(string phoneNumber)
    {
        try
        {
            var customerEntity = await _dbContext.Customers
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && !x.IsDeleted);

            return _mapper.Map<CustomerDM>(customerEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task AddElement(CustomerDM customerDataModel)
    {
        try
        {
            customerDataModel.Validate();

            var existingElement = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.ID == customerDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", customerDataModel.ID);
            }
            // Optional: Check for existing phone number if it should be unique
            var existingPhoneCustomer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == customerDataModel.PhoneNumber && !x.IsDeleted);
            if (existingPhoneCustomer != null)
            {
                throw new ElementExistsException("CustomerEntityPhoneNumber", customerDataModel.PhoneNumber);
            }

            var customerEntity = _mapper.Map<Customer>(customerDataModel);

            await _dbContext.Customers.AddAsync(customerEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException?.Message?.Contains("unique constraint") ?? false || (ex.InnerException?.GetType().Name == "PostgresException" && ex.InnerException.Message.Contains("23505")))
            {
                throw new ElementExistsException("CustomerINFO", "*[ID or PhoneNumber]");
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task UpdElement(CustomerDM customerDataModel)
    {
        try
        {
            customerDataModel.Validate();

            var element = await GetCustomerByID(customerDataModel.ID); // Find active
            if (element == null)
            {
                throw new ElementNotFoundException(customerDataModel.ID);
            }
            if (element.IsDeleted) // Prevent updating if soft-deleted
            {
                throw new ElementNotFoundException(customerDataModel.ID);
            }

            // Optional: Check if updated phone number is unique and belongs to a different active customer
            if (!string.IsNullOrEmpty(customerDataModel.PhoneNumber))
            {
                var existingPhoneCustomer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == customerDataModel.PhoneNumber && x.ID != customerDataModel.ID && !x.IsDeleted);
                if (existingPhoneCustomer != null)
                {
                    throw new ElementExistsException("CustomerEntityPhoneNumber", customerDataModel.PhoneNumber);
                }
            }

            _mapper.Map(customerDataModel, element);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException?.Message?.Contains("unique constraint") ?? false || (ex.InnerException?.GetType().Name == "PostgresException" && ex.InnerException.Message.Contains("23505")))
            {
                throw new ElementExistsException("CustomerINFO", "[PhoneNumber");
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task DelElement(string id)
    {
        try
        {
            var element = await GetCustomerByID(id); // Find active
            if (element == null)
            {
                throw new ElementNotFoundException(id);
            }

            element.IsDeleted = true;

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
            var element = await GetAnyCustomerByID(id); // Find any, including deleted

            if (element == null || !element.IsDeleted) // Check if found and *is* deleted
            {
                throw new ElementNotFoundException(id);
            }

            // Optional: Check if restoring would create a phone number conflict with an existing active customer
            if (!string.IsNullOrEmpty(element.PhoneNumber))
            {
                var existingPhoneCustomer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == element.PhoneNumber && x.ID != element.ID && !x.IsDeleted);
                if (existingPhoneCustomer != null)
                {
                    throw new ElementExistsException("CustomerEntityPhoneNumber", element.PhoneNumber);
                }
            }

            element.IsDeleted = false;
            _dbContext.Customers.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException?.Message?.Contains("unique constraint") ?? false || (ex.InnerException?.GetType().Name == "PostgresException" && ex.InnerException.Message.Contains("23505")))
            {
                // Specific handling if restoring causes a unique constraint violation (e.g., phone number)
                throw new ElementExistsException("CustomerEntityID", id);
            }
            throw new StorageException(ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    // Helper method to get an active customer entity by ID
    private Task<Customer?> GetCustomerByID(string id)
    {
        return _dbContext.Customers
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }

    // Helper method to get any customer entity (including deleted) by ID
    private Task<Customer?> GetAnyCustomerByID(string id)
    {
        return _dbContext.Customers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}