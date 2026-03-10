namespace RealEstate.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationToUserAsync(string userId, string title, string message, string type);
    Task SendNotificationToAgentAsync(string agentUserId, string title, string message, string type);
}
