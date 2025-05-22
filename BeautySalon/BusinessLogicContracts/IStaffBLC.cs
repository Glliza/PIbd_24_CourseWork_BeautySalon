using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.BusinessLogicContracts;

public interface IStaffBLC
{
    List<StaffDM> GetAllStaff(bool onlyActive = true);
    List<StaffDM> GetFilteredStaff(
        DateTime? fromBirthDate = null,
        DateTime? toBirthDate = null,
        DateTime? fromEmploymentDate = null,
        DateTime? toEmploymentDate = null,
        PostType? postType = null);
    StaffDM GetStaffById(string id);
    StaffDM GetStaffByFIO(string fio);
    void InsertStaff(StaffDM staffDataModel);
    void UpdateStaff(StaffDM staffDataModel);
    void DeleteStaff(string id);
}