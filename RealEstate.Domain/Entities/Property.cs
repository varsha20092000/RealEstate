namespace RealEstate.Domain.Entities;

public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal AreaSqFt { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string ListingType { get; set; } = "Sale";
    public string Status { get; set; } = "Available";
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Guid? AgentId { get; set; }
    public Agent? Agent { get; set; }

    public ICollection<PropertyImage> Images { get; set; }
        = new List<PropertyImage>();
    public ICollection<Inquiry> Inquiries { get; set; }
        = new List<Inquiry>();
    public ICollection<Favorite> Favorites { get; set; }
        = new List<Favorite>();
}