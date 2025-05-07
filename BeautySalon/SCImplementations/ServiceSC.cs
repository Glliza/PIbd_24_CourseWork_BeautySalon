using Microsoft.EntityFrameworkCore;
using BeautySalon.StorageContracts;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.Entities;
using AutoMapper;

namespace BeautySalon.SCImplementations;

internal class ServiceSC : IServiceSC
{
    private readonly SalonDbContext _dbContext;
    private readonly Mapper _mapper;

    public ServiceSC(SalonDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Service, ServiceDM>();
            cfg.CreateMap<ServiceDM, Service>()
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        });
        _mapper = new Mapper(config);
    }

    public async Task<List<ServiceDM>> GetList(bool onlyActive = true, string? name = null, int? minDurationMinutes = null, int? maxDurationMinutes = null, decimal? minBasePrice = null, decimal? maxBasePrice = null)
    {
        try
        {
            var query = _dbContext.Services.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (name is not null)
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (minDurationMinutes is not null)
            {
                query = query.Where(x => x.DurationMinutes >= minDurationMinutes.Value);
            }
            if (maxDurationMinutes is not null)
            {
                query = query.Where(x => x.DurationMinutes <= maxDurationMinutes.Value);
            }
            if (minBasePrice is not null)
            {
                query = query.Where(x => x.BasePrice >= minBasePrice.Value);
            }
            if (maxBasePrice is not null)
            {
                query = query.Where(x => x.BasePrice <= maxBasePrice.Value);
            }

            var serviceEntities = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ServiceDM>>(serviceEntities);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<ServiceDM?> GetElementByID(string id)
    {
        try
        {
            var serviceEntity = await GetServiceByID(id);
            return _mapper.Map<ServiceDM>(serviceEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task<ServiceDM?> GetElementByName(string name)
    {
        try
        {
            var serviceEntity = await _dbContext.Services
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted);

            return _mapper.Map<ServiceDM>(serviceEntity);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public async Task AddElement(ServiceDM serviceDataModel)
    {
        try
        {
            serviceDataModel.Validate();

            var existingElement = await _dbContext.Services.AsNoTracking().FirstOrDefaultAsync(x => x.ID == serviceDataModel.ID);
            if (existingElement != null)
            {
                throw new ElementExistsException("ID", serviceDataModel.ID);
            }

            var existingServiceByName = await _dbContext.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Name == serviceDataModel.Name && !x.IsDeleted);
            if (existingServiceByName != null)
            {
                throw new ElementExistsException("ServiceEntityNAME", serviceDataModel.Name);
            }

            var serviceEntity = _mapper.Map<Service>(serviceDataModel);
            serviceEntity.IsDeleted = false;


            await _dbContext.Services.AddAsync(serviceEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ServiceEntityName", constraintName);
            }
            throw new StorageException(ex);
        }
        catch (ValidationException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task UpdElement(ServiceDM serviceDataModel)
    {
        try
        {
            serviceDataModel.Validate();

            var element = await GetServiceByID(serviceDataModel.ID);
            if (element == null)
            {
                throw new ElementNotFoundException(serviceDataModel.ID);
            }

            if (element.IsDeleted)
            {
                throw new ElementNotFoundException(serviceDataModel.ID);
            }

            if (element.Name != serviceDataModel.Name)
            {
                var existingServiceByName = await _dbContext.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Name == serviceDataModel.Name && x.ID != serviceDataModel.ID && !x.IsDeleted);
                if (existingServiceByName != null)
                {
                    throw new ElementExistsException("ServiceEntityNAME", serviceDataModel.Name);
                }
            }

            _mapper.Map(serviceDataModel, element);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ServiceEntityID", serviceDataModel.ID);
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
            var element = await GetServiceByID(id);
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
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    public async Task RestoreElement(string id)
    {
        try
        {
            var element = await GetAnyServiceByID(id);

            if (element == null || !element.IsDeleted)
            {
                throw new ElementNotFoundException(id);
            }

            var existingServiceByName = await _dbContext.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Name == element.Name && x.ID != element.ID && !x.IsDeleted);
            if (existingServiceByName != null)
            {
                throw new ElementExistsException("ServiceEntityNAME", element.Name);
            }

            element.IsDeleted = false;

            _dbContext.Services.Attach(element);
            _dbContext.Entry(element).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            if (ex.InnerException != null && (ex.InnerException.Message.Contains("unique constraint") || (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")))
            {
                string constraintName = (ex.InnerException as Npgsql.PostgresException)?.ConstraintName ?? "Unknown Unique Constraint";
                throw new ElementExistsException("ServiceEntityID", id);
            }
            throw new StorageException(ex);
        }
        catch (ElementNotFoundException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (ElementExistsException) { _dbContext.ChangeTracker.Clear(); throw; }
        catch (Exception ex) { _dbContext.ChangeTracker.Clear(); throw new StorageException(ex); }
    }

    private Task<Service?> GetServiceByID(string id)
    {
        return _dbContext.Services
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }

    private Task<Service?> GetAnyServiceByID(string id)
    {
        return _dbContext.Services
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == id);
    }
}