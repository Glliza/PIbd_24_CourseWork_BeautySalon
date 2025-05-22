using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Visits")]
internal class Visit
{
    [Key]
    public required string ID { get; set; }

    [ForeignKey(nameof(CustomerID))]
    public required string CustomerID { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(StaffID))]
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!;

    [ForeignKey(nameof(RequestID))]
    public string? RequestID { get; set; }
    public virtual Request? Request { get; set; }

    public bool Status { get; set; }
    public DateTime DateTimeOfVisit { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<ServiceListItem>? Services { get; set; } = new List<ServiceListItem>();
}