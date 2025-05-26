using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts
{
    public interface IStaffAdapter
    {
        StaffR GetUserByLogin(string login);
        StaffR RegisterWorker(StaffVM staffModel);
        StaffR ChangeStaffInfo(StaffVM staffModel);
        StaffR GetAllStaff(bool onlyActive = true);
        StaffR GetStaffByFIO(string fio);
        StaffR GetStaffById(string id);
        StaffR CreateStaff(StaffVM staffModel);
        StaffR UpdateStaff(StaffVM staffModel);
        StaffR RemoveStaff(string id);
    }
}
