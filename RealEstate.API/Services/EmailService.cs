using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Settings;

namespace RealEstate.API.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    private async Task SendEmailAsync(string toEmail, string toName,
        string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress(toName, toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Host, _settings.Port,
            SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendInquiryNotificationAsync(string agentEmail,
        string agentName, string buyerName, string propertyTitle, string message)
    {
        var html = $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            <div style='background:linear-gradient(135deg,#1e40af,#3b82f6);
                padding:30px;text-align:center;border-radius:10px 10px 0 0;'>
                <h1 style='color:white;margin:0;'>🏠 RealEstate</h1>
            </div>
            <div style='background:#f8fafc;padding:30px;border-radius:0 0 10px 10px;'>
                <h2 style='color:#1e40af;'>New Inquiry Received!</h2>
                <p>Hello <strong>{agentName}</strong>,</p>
                <p>You have received a new inquiry for your property:</p>
                <div style='background:white;padding:20px;border-radius:8px;
                    border-left:4px solid #3b82f6;margin:20px 0;'>
                    <p><strong>Property:</strong> {propertyTitle}</p>
                    <p><strong>From:</strong> {buyerName}</p>
                    <p><strong>Message:</strong></p>
                    <p style='color:#64748b;'>{message}</p>
                </div>
                <p>Login to your dashboard to reply to this inquiry.</p>
                <a href='http://localhost:5000/Home/AgentDashboard'
                    style='background:#1e40af;color:white;padding:12px 24px;
                    border-radius:6px;text-decoration:none;display:inline-block;
                    margin-top:10px;'>View Dashboard</a>
                <hr style='margin:30px 0;border:none;border-top:1px solid #e2e8f0;'/>
                <p style='color:#94a3b8;font-size:12px;text-align:center;'>
                    RealEstate App — Connecting Buyers & Agents</p>
            </div>
        </div>";

        await SendEmailAsync(agentEmail, agentName,
            $"New Inquiry for {propertyTitle}", html);
    }

    public async Task SendInquiryReplyAsync(string buyerEmail, string buyerName,
        string agentName, string propertyTitle, string reply)
    {
        var html = $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            <div style='background:linear-gradient(135deg,#1e40af,#3b82f6);
                padding:30px;text-align:center;border-radius:10px 10px 0 0;'>
                <h1 style='color:white;margin:0;'>🏠 RealEstate</h1>
            </div>
            <div style='background:#f8fafc;padding:30px;border-radius:0 0 10px 10px;'>
                <h2 style='color:#1e40af;'>Agent Replied to Your Inquiry!</h2>
                <p>Hello <strong>{buyerName}</strong>,</p>
                <p>The agent has replied to your inquiry about:</p>
                <div style='background:white;padding:20px;border-radius:8px;
                    border-left:4px solid #22c55e;margin:20px 0;'>
                    <p><strong>Property:</strong> {propertyTitle}</p>
                    <p><strong>Agent:</strong> {agentName}</p>
                    <p><strong>Reply:</strong></p>
                    <p style='color:#64748b;'>{reply}</p>
                </div>
                <a href='http://localhost:5000/Home/MyInquiries'
                    style='background:#1e40af;color:white;padding:12px 24px;
                    border-radius:6px;text-decoration:none;display:inline-block;
                    margin-top:10px;'>View My Inquiries</a>
                <hr style='margin:30px 0;border:none;border-top:1px solid #e2e8f0;'/>
                <p style='color:#94a3b8;font-size:12px;text-align:center;'>
                    RealEstate App — Connecting Buyers & Agents</p>
            </div>
        </div>";

        await SendEmailAsync(buyerEmail, buyerName,
            $"Reply to Your Inquiry — {propertyTitle}", html);
    }

    public async Task SendVisitConfirmationAsync(string buyerEmail,
        string buyerName, string propertyTitle, DateTime visitDate, string notes)
    {
        var html = $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            <div style='background:linear-gradient(135deg,#1e40af,#3b82f6);
                padding:30px;text-align:center;border-radius:10px 10px 0 0;'>
                <h1 style='color:white;margin:0;'>🏠 RealEstate</h1>
            </div>
            <div style='background:#f8fafc;padding:30px;border-radius:0 0 10px 10px;'>
                <h2 style='color:#1e40af;'>Visit Confirmed! 🎉</h2>
                <p>Hello <strong>{buyerName}</strong>,</p>
                <p>Your property visit has been confirmed!</p>
                <div style='background:white;padding:20px;border-radius:8px;
                    border-left:4px solid #f59e0b;margin:20px 0;'>
                    <p><strong>Property:</strong> {propertyTitle}</p>
                    <p><strong>Visit Date:</strong>
                        {visitDate:dddd, MMMM dd yyyy} at {visitDate:hh:mm tt}</p>
                    <p><strong>Notes:</strong> {(string.IsNullOrEmpty(notes) ? "None" : notes)}</p>
                </div>
                <p>Please be on time for your visit. Contact the agent if you need to reschedule.</p>
                <a href='http://localhost:5000/Home/Profile'
                    style='background:#1e40af;color:white;padding:12px 24px;
                    border-radius:6px;text-decoration:none;display:inline-block;
                    margin-top:10px;'>View My Bookings</a>
                <hr style='margin:30px 0;border:none;border-top:1px solid #e2e8f0;'/>
                <p style='color:#94a3b8;font-size:12px;text-align:center;'>
                    RealEstate App — Connecting Buyers & Agents</p>
            </div>
        </div>";

        await SendEmailAsync(buyerEmail, buyerName,
            $"Visit Confirmed — {propertyTitle}", html);
    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        var html = $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            <div style='background:linear-gradient(135deg,#1e40af,#3b82f6);
                padding:30px;text-align:center;border-radius:10px 10px 0 0;'>
                <h1 style='color:white;margin:0;'>🏠 RealEstate</h1>
            </div>
            <div style='background:#f8fafc;padding:30px;border-radius:0 0 10px 10px;'>
                <h2 style='color:#1e40af;'>Welcome to RealEstate! 🎉</h2>
                <p>Hello <strong>{fullName}</strong>,</p>
                <p>Welcome to RealEstate — your one-stop platform for finding
                    your dream property!</p>
                <div style='background:white;padding:20px;border-radius:8px;
                    margin:20px 0;'>
                    <h3 style='color:#1e40af;margin-top:0;'>What you can do:</h3>
                    <p>🏠 Browse hundreds of properties</p>
                    <p>❤️ Save your favorite properties</p>
                    <p>📩 Send inquiries to agents</p>
                    <p>📅 Book property visits</p>
                </div>
                <a href='http://localhost:5000'
                    style='background:#1e40af;color:white;padding:12px 24px;
                    border-radius:6px;text-decoration:none;display:inline-block;
                    margin-top:10px;'>Start Browsing</a>
                <hr style='margin:30px 0;border:none;border-top:1px solid #e2e8f0;'/>
                <p style='color:#94a3b8;font-size:12px;text-align:center;'>
                    RealEstate App — Connecting Buyers & Agents</p>
            </div>
        </div>";

        await SendEmailAsync(email, fullName, "Welcome to RealEstate! 🏠", html);
    }
    public async Task SendPaymentConfirmationAsync(string buyerEmail,
    string buyerName, string propertyTitle, decimal amount, string paymentId)
    {
        var html = $@"
    <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
        <div style='background:linear-gradient(135deg,#1e40af,#3b82f6);
            padding:30px;text-align:center;border-radius:10px 10px 0 0;'>
            <h1 style='color:white;margin:0;'>🏠 RealEstate</h1>
        </div>
        <div style='background:#f8fafc;padding:30px;border-radius:0 0 10px 10px;'>
            <h2 style='color:#16a34a;'>Payment Successful! 💳✅</h2>
            <p>Hello <strong>{buyerName}</strong>,</p>
            <p>Your deposit payment has been received successfully!</p>
            <div style='background:white;padding:20px;border-radius:8px;
                border-left:4px solid #16a34a;margin:20px 0;'>
                <p><strong>Property:</strong> {propertyTitle}</p>
                <p><strong>Amount Paid:</strong> ₹{amount.ToString("N0")}</p>
                <p><strong>Payment ID:</strong> {paymentId}</p>
                <p><strong>Status:</strong> <span style='color:#16a34a;'>✅ Confirmed</span></p>
            </div>
            <p>Your visit booking is now confirmed. The agent will contact you shortly.</p>
            <a href='http://localhost:5000/Home/MyInquiries'
                style='background:#1e40af;color:white;padding:12px 24px;
                border-radius:6px;text-decoration:none;display:inline-block;
                margin-top:10px;'>View My Bookings</a>
            <hr style='margin:30px 0;border:none;border-top:1px solid #e2e8f0;'/>
            <p style='color:#94a3b8;font-size:12px;text-align:center;'>
                RealEstate App — Connecting Buyers & Agents</p>
        </div>
    </div>";

        await SendEmailAsync(buyerEmail, buyerName,
            $"Payment Confirmed — ₹{amount.ToString("N0")} Deposit Received", html);
    }
}