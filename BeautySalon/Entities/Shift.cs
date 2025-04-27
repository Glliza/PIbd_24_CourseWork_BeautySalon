using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Shifts")]
internal class Shift
{
    [Key]
    public required string ID { get; set; }

    // Foreign Key to CashBox
    [ForeignKey(nameof(CashBoxID))] // FK column name in Shifts table
    public required string CashBoxID { get; set; }
    public virtual CashBox CashBox { get; set; } = null!; // Navigation property (needs EF entity CashBox)

    // Foreign Key to Staff
    [ForeignKey(nameof(StaffID))] // FK column name in Shifts table
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!; // Navigation property (needs EF entity Staff)

    public DateTime DateTimeStart { get; set; }

    public DateTime? DateTimeFinish { get; set; } // Matches ShiftDM field name (Nullable)

    // IsDeleted flag (can represent being archived or historically closed, separate from DateTimeFinish)
    public bool IsDeleted { get; set; } = false; // Flag for soft deletion

    // Optional => REQUIRED: Navigation back to Receipts processed during this shift (1-to-Many)
    // You might link Receipts directly to Staff or Shift, depending on your process.
    // If linking to Shift:
    public virtual ICollection<Receipt>? Receipts { get; set; } = new List<Receipt>(); // Needs FK on Receipt
}
