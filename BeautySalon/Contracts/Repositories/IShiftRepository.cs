using BeautySalon.DataModels;
namespace BeautySalon.Contracts.Repositories;

public interface ISmenaRepository : IRepository<Smena>
{
    Task<Smena?> GetActiveSmena(int staffId);
    Task<IEnumerable<Smena>> GetSmenasForCashBox(int cashBoxId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Smena>> GetSmenasForStaff(int staffId, DateTime startDate, DateTime endDate);
}