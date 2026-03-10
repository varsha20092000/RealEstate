namespace RealEstate.Domain.Entities;

public class VisitBooking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime VisitDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }

    public string BuyerId { get; set; } = string.Empty;
    public AppUser? Buyer { get; set; }

    public Guid? AgentId { get; set; }
    public Agent? Agent { get; set; }
}