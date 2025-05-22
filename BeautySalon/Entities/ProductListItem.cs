using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("ProductListItems")]
internal class ProductListItem
{
    [Key]
    public required string ID { get; set; }

    [ForeignKey(nameof(ParentReceiptID))]
    public string? ParentReceiptID { get; set; }
    public virtual Receipt? ParentReceipt { get; set; }

    [ForeignKey(nameof(ParentRequestID))]
    public string? ParentRequestID { get; set; }
    public virtual Request? ParentRequest { get; set; }

    [ForeignKey(nameof(ProductID))]
    public required string ProductID { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int Amount { get; set; }
    public bool IsDeleted { get; set; } = false;
}