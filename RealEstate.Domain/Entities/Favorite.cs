namespace RealEstate.Domain.Entities;

public class Favorite
{
    public int Id { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }

    public string UserId { get; set; } = string.Empty;
    public AppUser? User { get; set; }
}