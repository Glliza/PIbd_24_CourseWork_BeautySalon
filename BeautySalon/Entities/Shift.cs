using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Shifts")]
internal class Shift
{
    [Key]
    public required string ID { get; set; }

    [ForeignKey(nameof(CashBoxID))] 
    public required string CashBoxID { get; set; }
    public virtual CashBox CashBox { get; set; } = null!;

    [ForeignKey(nameof(StaffID))]
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!;
    public DateTime DateTimeStart { get; set; }
    public DateTime? DateTimeFinish { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<Receipt>? Receipts { get; set; } = new List<Receipt>();
}
