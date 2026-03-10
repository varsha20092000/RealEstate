using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using System.Security.Claims;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiriesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationService _notificationService;

    public InquiriesController(IUnitOfWork uow, IEmailService emailService,
        UserManager<AppUser> userManager, INotificationService notificationService)
    {
        _uow = uow;
        _emailService = emailService;
        _userManager = userManager;
        _notificationService = notificationService;
    }
    // GET api/inquiries
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inquiries = await _uow.Inquiries.GetAllAsync();
        return Ok(inquiries);
    }

    // GET api/inquiries/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var inquiry = await _uow.Inquiries.GetByIdAsync(id);
        if (inquiry == null) return NotFound("Inquiry not found");
        return Ok(inquiry);
    }

    // POST api/inquiries
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Inquiry inquiry)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        inquiry.BuyerId = userId;
        inquiry.CreatedAt = DateTime.UtcNow;
        inquiry.IsReplied = false;

        await _uow.Inquiries.AddAsync(inquiry);
        await _uow.SaveChangesAsync();

        // Send email notification to agent
        try
        {
            var property = await _uow.Properties.GetByIdAsync(inquiry.PropertyId);
            var buyer = await _userManager.FindByIdAsync(userId);

            if (property != null && buyer != null && property.AgentId != null)
            {
                var agent = await _uow.Agents.GetByIdAsync(property.AgentId.Value);
                if (agent != null)
                {
                    var agentUser = await _userManager.FindByIdAsync(agent.UserId);
                    if (agentUser != null)
                    {
                        await _emailService.SendInquiryNotificationAsync(
                            agentUser.Email!,
                            agentUser.FullName,
                            buyer.FullName,
                            property.Title,
                            inquiry.Message);
                    }
                }
            }
        }
        catch { /* Don't fail if email fails */ }

        // Send real-time notification to agent
        try
        {
            var property = await _uow.Properties.GetByIdAsync(inquiry.PropertyId);
            if (property?.AgentId != null)
            {
                var agent = await _uow.Agents.GetByIdAsync(property.AgentId.Value);
                if (agent != null)
                {
                    await _notificationService.SendNotificationToAgentAsync(
                        agent.UserId,
                        "New Inquiry Received!",
                        $"New inquiry for {property.Title}",
                        "inquiry");
                }
            }
        }
        catch { /* Don't fail if notification fails */ }

        return CreatedAtAction(nameof(GetById), new { id = inquiry.Id }, inquiry);
    }

    // PUT api/inquiries/{id}/reply
    [Authorize(Roles = "Agent")]
    [HttpPut("{id}/reply")]
    public async Task<IActionResult> Reply(Guid id, [FromBody] string reply)
    {
        var inquiry = await _uow.Inquiries.GetByIdAsync(id);
        if (inquiry == null) return NotFound("Inquiry not found");

        inquiry.Reply = reply;
        inquiry.IsReplied = true;
        inquiry.RepliedAt = DateTime.UtcNow;

        await _uow.Inquiries.UpdateAsync(inquiry);
        await _uow.SaveChangesAsync();

        // Send email notification to buyer
        try
        {
            var property = await _uow.Properties.GetByIdAsync(inquiry.PropertyId);
            var buyer = await _userManager.FindByIdAsync(inquiry.BuyerId);
            var agentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var agentUser = await _userManager.FindByIdAsync(agentUserId!);

            if (property != null && buyer != null && agentUser != null)
            {
                await _emailService.SendInquiryReplyAsync(
                    buyer.Email!,
                    buyer.FullName,
                    agentUser.FullName,
                    property.Title,
                    reply);
            }
        }
        catch { /* Don't fail if email fails */ }

        // Send real-time notification to buyer
        try
        {
            var property = await _uow.Properties.GetByIdAsync(inquiry.PropertyId);
            await _notificationService.SendNotificationToUserAsync(
                inquiry.BuyerId,
                "Agent Replied!",
                $"Your inquiry about {property?.Title} has been answered!",
                "reply");
        }
        catch { /* Don't fail if notification fails */ }

        return Ok(inquiry);
    }
}