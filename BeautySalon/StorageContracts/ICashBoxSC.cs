using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface ICashBoxSC
{
    Task<List<CashBoxDM>> GetList(bool onlyActive = true); 
    Task<CashBoxDM?> GetElementByID(string id);
    Task AddElement(CashBoxDM cashBoxDataModel);
    Task UpdElement(CashBoxDM cashBoxDataModel);
    Task DelElement(string id);
}
