
using System.Collections.Generic;

namespace BeautySalon.DataModels
{
    public class Product(string id, string naming, int amount, double price, string note)
    {
        public string Id { get; private set; } = id;
        public string Naming { get; private set; } = naming;
        public string Note { get; private set; } = note;
        public int Amount { get; private set; } = amount;
        public double Price { get; private set; } = price;
    }
}
