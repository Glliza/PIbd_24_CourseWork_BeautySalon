
namespace BeautySalon.DataModels
{
    public class Customer(string id, string fio, string phoneNumber)
    {
        public string Id { get; private set; } = id;
        public string FIO { get; private set; } = fio;
        public string PhoneNumber { get; private set; } = phoneNumber;
    }
}
