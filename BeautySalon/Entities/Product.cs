using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.Enums;
using BeautySalon.DataModels;

namespace BeautySalon.Entities;

[Table("Products")]
internal class Product
{
    [Key]
    public required string ID { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    public decimal PricePerOne { get; set; }

    // Assuming ProductType enum is also relevant at the database level for filtering/categorization
    // If so, map it in DbContext OnModelCreating
    public required ProductType Type { get; set; } // Example: Using the ProductType enum from Core

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties (Inverse relationships where other entities have an FK to Product)
    // ProductListItem has a FK to Product
    public virtual ICollection<ProductListItem>? ProductListItems { get; set; } = new List<ProductListItem>();

    // [ ? ] might also link directly to Product
}
