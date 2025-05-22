using BeautySalon.Infrastructure;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts.OperationResponses;

public class CashBoxR : OperationResponseBase
{
    public static CashBoxR OK(List<CashBoxVM> data) => OK<CashBoxR, List<CashBoxVM>>(data);

    public static CashBoxR OK(CashBoxVM data) => OK<CashBoxR, CashBoxVM>(data);

    public static CashBoxR NoContent() => NoContent<CashBoxR>();

    public static CashBoxR BadRequest(string message) => BadRequest<CashBoxR>(message);

    public static CashBoxR NotFound(string message) => NotFound<CashBoxR>(message);

    public static CashBoxR InternalServerError(string message) => InternalServerError<CashBoxR>(message);
}