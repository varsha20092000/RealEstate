namespace RealEstate.Domain.Entities;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Guid AgentId { get; set; }
    public Agent? Agent { get; set; }

    public string ReviewerId { get; set; } = string.Empty;
    public AppUser? Reviewer { get; set; }
}