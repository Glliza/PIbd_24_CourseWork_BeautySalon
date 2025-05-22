using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogicContracts;

public interface ICashBoxBLC
{
    List<CashBoxDM> GetAllCashBoxes(bool onlyActive = true);
    CashBoxDM GetCashBoxById(string id);
    void InsertCashBox(CashBoxDM cashBoxDataModel);
    void UpdateCashBox(CashBoxDM cashBoxDataModel);
    void DeleteCashBox(string id);
}