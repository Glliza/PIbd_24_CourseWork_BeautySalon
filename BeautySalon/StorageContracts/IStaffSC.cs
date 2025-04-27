using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.StorageContracts;

public interface IStaffSC
{
    Task<List<StaffDM>> GetList(bool onlyActive = true, string? staffID = null,
        DateTime? fromBirthDate = null, DateTime? toBirthDate = null,
        DateTime? fromEmploymentDate = null, DateTime? toEmploymentDate = null,
        PostType? postType = null); // Added PostType filter

    Task<StaffDM?> GetElementByID(string id);

    Task<StaffDM?> GetElementByFIO(string fio);

    Task AddElement(StaffDM staffDataModel);

    Task UpdElement(StaffDM staffDataModel);

    Task DelElement(string id);

    Task RestoreElement(string id);
}
