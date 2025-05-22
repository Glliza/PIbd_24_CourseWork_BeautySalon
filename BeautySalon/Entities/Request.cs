using BeautySalon.DataModels;
using BeautySalon.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("Requests")]
internal class Request
{
    [Key]
    public required string ID { get; set; }

    [ForeignKey(nameof(CustomerID))]
    public required string CustomerID { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public DateTime DateCreated { get; set; }
    public required OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<ProductListItem> Products { get; set; } = new List<ProductListItem>();
    public virtual ICollection<ServiceListItem> Services { get; set; } = new List<ServiceListItem>();
    public virtual ICollection<Visit>? Visits { get; set; } = new List<Visit>();
}