using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.Enums;

namespace BeautySalon.Entities;

[Table("Workers")]
internal class Staff
{
    [Key]
    public required string ID { get; set; }
    public required string FIO { get; set; }
    public required PostType postType { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime EmploymentDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    [ForeignKey("StaffID")]
    public virtual ICollection<Shift>? Shifts { get; set; }

    [ForeignKey("StaffID")]
    public virtual ICollection<Receipt>? Receipts { get; set; }

    [ForeignKey("StaffID")] 
    public virtual ICollection<Visit>? Visits { get; set; }
}

