using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.Contracts.Repositories;

public interface IStaffRepository : IRepository<Staff>
{
    Task<IEnumerable<Staff>> GetStaffByPostType(PostType postType);
    Task<IEnumerable<Staff>> GetAvailableStaff(DateTime dateTime, int? durationMinutes); // finding available staff based on visits/shifts
    Task<IEnumerable<Staff>> FindStaffByName(string searchName);
}
