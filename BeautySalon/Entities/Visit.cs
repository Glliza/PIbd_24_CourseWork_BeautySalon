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

    // Foreign Key to Request (nullable, as Visit can potentially exist without a direct Request?)
    // Added RequestID based on the DbContext mapping from Request to Many Visits
    [ForeignKey(nameof(RequestID))]
    public string? RequestID { get; set; }
    public virtual Request? Request { get; set; }

    // Removed Properties related to ProductListHeader and ServiceListHeader [ ! ]

    public bool Status { get; set; }
    public DateTime DateTimeOfVisit { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation property for ServiceListItems (1 Visit to Many ServiceListItems)
    // Added based on ServiceListItem having ParentVisitID and DbContext mapping
    public virtual ICollection<ServiceListItem>? ServiceItems { get; set; } = new List<ServiceListItem>();
}
