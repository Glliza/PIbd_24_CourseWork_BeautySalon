namespace BeautySalon.ViewModels;

public class ServiceVM
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required decimal BasePrice { get; set; }
    public required int DurationMinutes { get; set; }
    public string? Description { get; set; }
}