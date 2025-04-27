using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Services")]
internal class Service
{
    [Key]
    public required string ID { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsDeleted { get; set; } = false; // Flag for soft deletion

    // Navigation Properties (Inverse relationships where other entities have an FK to Service)
    // ServiceListItem has a FK to Service
    public virtual ICollection<ServiceListItem>? ServiceListItems { get; set; } = new List<ServiceListItem>();

    // ... line items might also link directly to Service * (as well as for Products)

    // Note: The navigation properties are marked 'virtual'.
}
