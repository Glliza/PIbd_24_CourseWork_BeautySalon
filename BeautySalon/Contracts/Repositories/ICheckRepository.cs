using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories;

public interface ICheckRepository : IRepository<Check>
{
    // GetCheckWithItems => load CheckItems with Quantity and TotalPrice

    Task<Check?> GetCheckWithItems(int checkId);
    Task<IEnumerable<Check>> GetChecksByDateRange(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Check>> GetChecksByCustomerID(int customerId);
    Task<Check?> GetCheckByVisitID(int visitId);
}