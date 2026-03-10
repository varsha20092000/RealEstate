namespace RealEstate.Domain.Entities;

public class PropertyImage
{
    public int Id { get; set; }
    public Guid PropertyId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation
    public Property? Property { get; set; }
}