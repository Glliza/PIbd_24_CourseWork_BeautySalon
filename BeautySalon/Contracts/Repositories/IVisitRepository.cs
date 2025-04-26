using BeautySalon.DataModels;

namespace BeautySalon.Contracts.Repositories
{
    public interface IVisitRepository : IRepository<Visit>
    {
        Task<IEnumerable<Visit>> GetVisitsByStaffAndDate(int staffId, DateTime date);
        Task<IEnumerable<Visit>> GetUpcomingVisitsForCustomer(int customerId, DateTime? fromDate = null);
        Task<Visit?> GetVisitWithDetails(int visitId);

        // Method signature changes to use int for status
        // Consumer must know what integer values represent visit status (0, 1)
        Task<IEnumerable<Visit>> GetVisitsByStatus(int status); // Status: 0=Not Completed, 1=Completed
    }
}
