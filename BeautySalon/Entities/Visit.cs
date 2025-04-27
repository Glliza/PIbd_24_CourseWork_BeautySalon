using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.DataModels;

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

    [ForeignKey(nameof(ProductListID))] 
    public string? ProductListID { get; set; }
    public virtual ProductListHeader? ProductListHeader { get; set; } 

    [ForeignKey(nameof(ServiceListID))] 
    public required string ServiceListID { get; set; }
    public virtual ServiceListHeader ServiceListHeader { get; set; } = null!;
    public bool Status { get; set; }
    public DateTime DateTimeOfVisit { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual Receipt? Receipt { get; set; }
}
