using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface ICashBoxAdapter
{
    CashBoxR GetCashBoxById(string id);
    CashBoxR CreateCashBox(CashBoxVM cashBoxModel);
    CashBoxR UpdateCashBox(CashBoxVM cashBoxModel);
    CashBoxR DeleteCashBox(string id);
    CashBoxR GetAllCashBoxes(bool onlyActive = true);
}