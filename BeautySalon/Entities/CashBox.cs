using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("CashBoxes")]
internal class CashBox // Use internal as it's a DataAccess detail
{
    [Key] // Designates ID as the Primary Key
    public required string ID { get; set; } // Use public set for EF Core hydration (string GUID)

    public decimal CurrentCapacity { get; set; }

    // IsDeleted flag (optional for CashBox, might represent being decommissioned)
    public bool IsDeleted { get; set; } = false; // Flag for soft deletion

    // Navigation Property for the shifts linked to this cash box (1-to-Many)
    public virtual ICollection<Shift>? Shifts { get; set; } = new List<Shift>();

    // Optional: Navigation back to Receipts processed via this cash box (1-to-Many)
    // public virtual ICollection<Receipt>? Receipts { get; set; } = new List<Receipt>();
    // Needs FK on Receipt
}
