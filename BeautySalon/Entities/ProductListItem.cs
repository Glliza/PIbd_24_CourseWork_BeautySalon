using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("ProductListItems")]
internal class ProductListItem
{
    [Key]
    public required string ID { get; set; }

    // Foreign Key to the parent Receipt (Nullable, as it can belong to a Request instead)
    [ForeignKey(nameof(ParentReceiptID))]
    public string? ParentReceiptID { get; set; } // Renamed for clarity
    public virtual Receipt? ParentReceipt { get; set; }

    // Foreign Key to the parent Request (Nullable, as it can belong to a Receipt instead)
    [ForeignKey(nameof(ParentRequestID))]
    public string? ParentRequestID { get; set; } // Renamed for clarity
    public virtual Request? ParentRequest { get; set; } // Navigation property

    // Foreign Key to the Product being listed
    [ForeignKey(nameof(ProductID))] // FK column name
    public required string ProductID { get; set; }
    public virtual Product Product { get; set; } = null!; // Navigation property

    public int Amount { get; set; }

    // Include other properties needed for storage that might not be in the core DM,
    // e.g., PriceAtTimeOfSale, calculated TotalPrice for this line.
    // Based on your ReceiptDM/ProductListDM having TotalSumm at the item level:
    // public decimal TotalItemSumm { get; set; } // Example field

    // Add IsDeleted flag if individual list items can be soft-deleted
    public bool IsDeleted { get; set; } = false;

    // Note: Need a CHECK constraint in DbContext to ensure exactly one of ParentReceiptID or ParentRequestID is non-null.
}

