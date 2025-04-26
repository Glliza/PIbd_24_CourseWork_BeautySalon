using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories;

public interface ICheckItemRepository : IRepository<CheckItem>
{
    // Methods will return/accept CheckItem objects as defined by you (ID, CheckID, ServiceID, ProductID, Quantity, TotalPrice)
    Task<IEnumerable<CheckItem>> GetItemsForCheck(int checkId);
}