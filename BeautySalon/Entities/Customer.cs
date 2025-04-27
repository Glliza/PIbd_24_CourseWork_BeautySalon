using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Customers")]
internal class Customer
{
    [Key]
    public required string ID { get; set; }

    public required string FIO { get; set; }

    public DateTime BirthDate { get; set; }

    // PhoneNumber index configured in DbContext OnModelCreating
    public string? PhoneNumber { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties (EF Core uses these for relationships)
    // A Customer can be associated with many Receipts
    public virtual ICollection<Receipt>? Receipts { get; set; } = new List<Receipt>();

    // A Customer can be associated with many Requests (Orders)
    public virtual ICollection<Request>? Requests { get; set; } = new List<Request>();

    // A Customer can be associated with many Visits
    public virtual ICollection<Visit>? Visits { get; set; } = new List<Visit>();

    // Note: The navigation properties are marked 'virtual' for potential lazy loading,
    // but explicit/select loading with .Include() or .Select() is often preferred.
}
