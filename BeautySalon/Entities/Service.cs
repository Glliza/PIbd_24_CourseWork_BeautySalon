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
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<ServiceListItem>? ServiceListItems { get; set; } = new List<ServiceListItem>();
}