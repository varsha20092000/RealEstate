using Microsoft.AspNetCore.SignalR;
using RealEstate.Application.Interfaces;
using RealEstate.API.Hubs;

namespace RealEstate.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(string userId, string title,
        string message, string type)
    {
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", new
        {
            title,
            message,
            type,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendNotificationToAgentAsync(string agentUserId, string title,
        string message, string type)
    {
        await _hubContext.Clients.Group(agentUserId).SendAsync("ReceiveNotification", new
        {
            title,
            message,
            type,
            timestamp = DateTime.UtcNow
        });
    }
}