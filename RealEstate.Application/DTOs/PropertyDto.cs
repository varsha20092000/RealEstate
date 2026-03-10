namespace RealEstate.Application.DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal AreaSqFt { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string ListingType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AgentName { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid? AgentId { get; set; }
}

public class CreatePropertyDto
{
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
    public bool IsFeatured { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

}

public class UpdatePropertyDto : CreatePropertyDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Available";
    public Guid? AgentId { get; set; }
}