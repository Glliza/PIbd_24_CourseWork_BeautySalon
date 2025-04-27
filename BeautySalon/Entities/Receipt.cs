using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Recepies")]
internal class Receipt
{
    [Key]
    public required string ID { get; set; }

    // Foreign Key to Staff (who processed the receipt)
    [ForeignKey(nameof(StaffID))] // FK column name in Recepies table (matches ReceiptDM field name)
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!; // Navigation property

    // Foreign Key to Customer (who paid) - Nullable
    [ForeignKey(nameof(CustomerID))] // FK column name in Recepies table (matches ReceiptDM field name)
    public string? CustomerID { get; set; }
    public virtual Customer? Customer { get; set; } // Navigation property

    public DateTime DateIssued { get; set; }

    public decimal TotalSumm { get; set; }

    public bool IsCanceled { get; set; } = false;

    // Navigation Property for the product items (1-to-Many relationship)
    // This maps to the List<ProductListDM> (now List<ProductListItemDM>) in your ReceiptDM
    public virtual ICollection<ProductListItem> Products { get; set; } = new List<ProductListItem>(); // Matches ReceiptDM property name

    // Foreign Key to CashBox (Missing in your latest ReceiptDM template, but crucial domain concept)
    // Assuming CashBoxID FK in Recepies table points to CashBox.ID
    [ForeignKey(nameof(CashBoxID))] // FK column name in Recepies table
    public required string CashBoxID { get; set; } // Added based on previous discussions (CashBoxDM)
    public virtual CashBox CashBox { get; set; } = null!; // Navigation property (needs EF entity CashBox)

    // Foreign Key to Visit (Optional link back to the visit this receipt is for)
    // Assuming VisitID FK in Recepies table points to Visit.ID (Nullable)
    [ForeignKey(nameof(VisitID))] // FK column name in Recepies table
    public string? VisitID { get; set; } // Added based on previous discussions (VisitDM includes Receipt link)
    public virtual Visit? Visit { get; set; } // Navigation property (needs EF entity Visit)

    // Note: If ReceiptDM included ServiceListDM (now ServiceListItemDM), you'd add that ICollection here.
}