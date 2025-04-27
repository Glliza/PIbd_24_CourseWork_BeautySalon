using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("CashBoxes")]
internal class CashBox
{
    [Key]
    public required string ID { get; set; }
    public decimal CurrentCapacity { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<Shift>? Shifts { get; set; } = new List<Shift>();
}
