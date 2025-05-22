using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Entities;

[Table("ServiceListItems")]
internal class ServiceListItem
{
    [Key]
    public required string ID { get; set; }

    [ForeignKey(nameof(ParentRequestID))]
    public string? ParentRequestID { get; set; } 
    public virtual Request? ParentRequest { get; set; }

    [ForeignKey(nameof(ParentVisitID))]
    public string? ParentVisitID { get; set; }
    public virtual Visit? ParentVisit { get; set; }

    [ForeignKey(nameof(ServiceID))]
    public required string ServiceID { get; set; }
    public virtual Service Service { get; set; } = null!;
    public int QuantityOrSessions { get; set; }
    public int TotalItemDuration { get; set; }
    public decimal TotalItemPrice { get; set; }
    public bool IsDeleted { get; set; } = false;
}