using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("ServiceListItems")]
internal class ServiceListItem
{
    [Key]
    public required string ID { get; set; }

    // Foreign Key to the parent Request (Nullable, as it can belong to a Visit instead)
    [ForeignKey(nameof(ParentRequestID))]
    public string? ParentRequestID { get; set; } // Renamed for clarity
    public virtual Request? ParentRequest { get; set; } // Navigation property

    // Foreign Key to the parent Visit (Nullable, as it can belong to a Request instead)
    [ForeignKey(nameof(ParentVisitID))]
    public string? ParentVisitID { get; set; } // Renamed for clarity
    public virtual Visit? ParentVisit { get; set; } // Navigation property

    // Foreign Key to the Service being listed
    [ForeignKey(nameof(ServiceID))] // FK column name
    public required string ServiceID { get; set; }
    public virtual Service Service { get; set; } = null!; // Navigation property

    public int QuantityOrSessions { get; set; }

    public int TotalItemDuration { get; set; }

    public decimal TotalItemPrice { get; set; }

    // Add IsDeleted flag if individual list items can be soft-deleted
    public bool IsDeleted { get; set; } = false;

    // Note: Need a CHECK constraint in DbContext to ensure exactly one of ParentRequestID or ParentVisitID is non-null.
}
