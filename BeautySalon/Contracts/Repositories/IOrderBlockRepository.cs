using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories;

public interface IOrderBlockRepository : IRepository<OrderBlock>
{
    // Methods will return/accept OrderBlock objects as defined by you (ID, OrderID, ServiceID, ProductID, TotalSumm)
    Task<IEnumerable<OrderBlock>> GetBlocksForOrder(int orderId);
}