namespace RealEstate.Domain.Entities;

public class Inquiry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public string? Reply { get; set; }
    public bool IsReplied { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RepliedAt { get; set; }

    // Navigation
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }

    public string BuyerId { get; set; } = string.Empty;
    public AppUser? Buyer { get; set; }

    public Guid? AgentId { get; set; }
    public Agent? Agent { get; set; }
}