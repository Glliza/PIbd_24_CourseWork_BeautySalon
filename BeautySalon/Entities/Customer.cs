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
    public string? PhoneNumber { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<Receipt>? Receipts { get; set; } = new List<Receipt>();
    public virtual ICollection<Request>? Requests { get; set; } = new List<Request>();
    public virtual ICollection<Visit>? Visits { get; set; } = new List<Visit>();
}