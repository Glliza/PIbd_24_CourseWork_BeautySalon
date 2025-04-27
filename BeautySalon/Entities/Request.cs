using BeautySalon.DataModels;
using BeautySalon.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Requests")]
internal class Request
{
    [Key] // Designates ID as the Primary Key
    public required string ID { get; set; }

    // Foreign Key to Customer
    [ForeignKey(nameof(CustomerID))] // FK column name in Requests table
    public required string CustomerID { get; set; }
    public virtual Customer Customer { get; set; } = null!; // Navigation property

    public DateTime DateCreated { get; set; }

    // Status will be mapped to the OrderStatus enum in DbContext
    public required OrderStatus Status { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties for the list items (1-to-Many relationships)
    // These map to the List<> properties in your RequestDM
    public virtual ICollection<ProductListItem> ProductItems { get; set; } = new List<ProductListItem>();
    public virtual ICollection<ServiceListItem> ServiceItems { get; set; } = new List<ServiceListItem>();

    // Optional: Navigation back to Visits created from this request (1-to-Many)
    public virtual ICollection<Visit>? Visits { get; set; } = new List<Visit>(); // Needs FK on Visit entity
}
