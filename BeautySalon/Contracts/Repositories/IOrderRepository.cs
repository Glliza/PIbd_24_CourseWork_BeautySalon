using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.Contracts.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    // GetOrderWithBlocks will now load OrderBlocks with TotalSumm

    Task<Order?> GetOrderWithBlocks(int orderId);
    Task<IEnumerable<Order>> GetCustomerOrders(int customerId);
    Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status); // OrderStatus enum is kept
}
