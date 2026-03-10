using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstate.Application.Interfaces;

public interface IEmailService
{
    Task SendInquiryNotificationAsync(string agentEmail, string agentName,
        string buyerName, string propertyTitle, string message);

    Task SendInquiryReplyAsync(string buyerEmail, string buyerName,
        string agentName, string propertyTitle, string reply);

    Task SendVisitConfirmationAsync(string buyerEmail, string buyerName,
        string propertyTitle, DateTime visitDate, string notes);

    Task SendWelcomeEmailAsync(string email, string fullName);

    Task SendPaymentConfirmationAsync(string buyerEmail, string buyerName,
    string propertyTitle, decimal amount, string paymentId);
}
