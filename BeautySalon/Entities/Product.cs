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
    public required ProductType Type { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<ProductListItem>? ProductListItems { get; set; } = new List<ProductListItem>();
}
