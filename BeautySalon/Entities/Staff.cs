using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.Enums;

namespace BeautySalon.Entities;

[Table("Workers")] // table name from DbContext template [ ! ] *
internal class Staff
{
    [Key]
    public required string ID { get; set; }

    public required string FIO { get; set; }

    // Map enum to underlying type (e.g., int or string) in DbContext OnModelCreating
    public required PostType postType { get; set; }

    public DateTime BirthDate { get; set; }

    public DateTime EmploymentDate { get; set; }

    public bool IsDeleted { get; set; } = false;

    [ForeignKey("StaffID")] // FK column name in the Receipts table pointing to this Staff
    public virtual ICollection<Receipt>? Receipts { get; set; } // Renamed from Recepies for clarity and consistency with ReceiptDM

    [ForeignKey("WorkerId")] // FK column name in the Visits table pointing to this Staff
    public virtual ICollection<Visit>? Visits { get; set; } // Matching your template's WorkerId FK name

    // Note: The navigation properties are marked 'virtual' for lazy loading (though explicit/select loading is often preferred)
}

