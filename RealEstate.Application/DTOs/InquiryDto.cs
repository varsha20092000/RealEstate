namespace RealEstate.Application.DTOs;

public class CreateInquiryDto
{
    public Guid PropertyId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ReplyInquiryDto
{
    public Guid InquiryId { get; set; }
    public string Reply { get; set; } = string.Empty;
}

public class InquiryDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Reply { get; set; }
    public bool IsReplied { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
}