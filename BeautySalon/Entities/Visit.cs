using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.DataModels;

namespace BeautySalon.Entities;

[Table("Visits")]
internal class Visit
{
    [Key]
    public required string ID { get; set; } // Use public set for EF Core hydration (string GUID)

    // Foreign Key to Customer
    [ForeignKey(nameof(CustomerID))] // FK column name in Visits table
    public required string CustomerID { get; set; }
    public virtual Customer Customer { get; set; } = null!; // Navigation property

    [ForeignKey(nameof(StaffID))] // FK column name in Visits table
    public required string StaffID { get; set; }
    public virtual Staff Staff { get; set; } = null!; // Navigation property

    // Foreign Key to ProductListHeader (Nullable)
    // Assuming VisitDM.ProductListID points to a ProductListHeader entity's ID [ ! ]
    [ForeignKey(nameof(ProductListID))] // FK column name in Visits table
    public string? ProductListID { get; set; }
    public virtual ProductListHeader? ProductListHeader { get; set; } // Navigation property (needs EF entity ProductListHeader)

    // Foreign Key to ServiceListHeader (Required)
    // Assuming VisitDM.ServiceListID points to a ServiceListHeader entity's ID [ ! ]
    [ForeignKey(nameof(ServiceListID))] // FK column name in Visits table
    public required string ServiceListID { get; set; }
    public virtual ServiceListHeader ServiceListHeader { get; set; } = null!; // Navigation property (needs EF entity ServiceListHeader)

    public bool Status { get; set; }

    public DateTime DateTimeOfVisit { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsDeleted { get; set; } = false; // Flag for soft deletion

    // Navigation Property back to Check/Receipt (1-to-0..1)
    // Assuming a Receipt can be associated with this Visit (FK on Receipt pointing to Visit)
    public virtual Receipt? Receipt { get; set; } // Navigation property (needs EF entity Receipt)

    // NOT NEEDED, BUT: :)
    // Note: If Visits can link to Requests/Orders, you'd add a nullable RequestID FK here
    // and a navigation property to the Request entity.
    // [ForeignKey(nameof(RequestID))] public string? RequestID { get; set; }
    // public virtual Request? Request { get; set; }

    // If VisitDM's List<...> properties were to be mapped as 1-to-Many relationships instead of FKs to Headers:
    // public virtual ICollection<ProductListItem> ProductItems { get; set; } = new List<ProductListItem>();
    // public virtual ICollection<ServiceListItem> ServiceItems { get; set; } = new List<ServiceListItem>();

}
