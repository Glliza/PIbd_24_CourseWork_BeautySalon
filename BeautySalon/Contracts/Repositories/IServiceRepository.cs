using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories;

public interface IServiceRepository : IRepository<Service>
{
    Task<Service?> GetServiceByName(string name);
    Task<IEnumerable<Service>> GetServicesByIDs(IEnumerable<int> ids);
}