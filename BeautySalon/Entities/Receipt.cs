using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Recepies")]
internal class Receipt
{
    [Key]
    public required string ID { get; set; }
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!;
    public string? CustomerID { get; set; }
    public virtual Customer? Customer { get; set; }
    public DateTime DateIssued { get; set; }
    public decimal TotalSumm { get; set; }
    public bool IsCanceled { get; set; } = false;
    public virtual ICollection<ProductListItem> Products { get; set; } = new List<ProductListItem>();

    [ForeignKey(nameof(CashBoxID))] 
    public required string CashBoxID { get; set; } 
    public virtual CashBox CashBox { get; set; } = null!;

    [ForeignKey(nameof(VisitID))]
    public string? VisitID { get; set; } 
    public virtual Visit? Visit { get; set; } 
}