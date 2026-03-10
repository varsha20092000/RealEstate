namespace RealEstate.Domain.Entities;

public class Agent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string LicenseNumber { get; set; } = string.Empty;
    public string AgencyName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public decimal AvgRating { get; set; }
    public int TotalSales { get; set; }
    public bool IsApproved { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public string UserId { get; set; } = string.Empty;
    public AppUser? User { get; set; }
    public ICollection<Property> Properties { get; set; }
        = new List<Property>();
}