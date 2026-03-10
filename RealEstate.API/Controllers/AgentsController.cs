using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using System.Security.Claims;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public AgentsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET api/agents
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var agents = await _uow.Agents.GetAllAsync();
        return Ok(agents);
    }

    // GET api/agents/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var agent = await _uow.Agents.GetByIdAsync(id);
        if (agent == null) return NotFound("Agent not found");
        return Ok(agent);
    }

    // POST api/agents
    [Authorize(Roles = "Admin,Agent")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Agent agent)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        agent.UserId = userId;
        await _uow.Agents.AddAsync(agent);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = agent.Id }, agent);
    }

    // PUT api/agents/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Agent agent)
    {
        if (id != agent.Id) return BadRequest();
        await _uow.Agents.UpdateAsync(agent);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
    // GET api/agents/analytics
    [Authorize(Roles = "Agent")]
    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        // Get agent profile
        var agents = await _uow.Agents.GetAllAsync();
        var agent = agents.FirstOrDefault(a => a.UserId == userId);
        if (agent == null) return NotFound("Agent profile not found");

        // Get agent's properties
        var allProperties = await _uow.Properties.GetAllAsync();
        var myProperties = allProperties.Where(p => p.AgentId == agent.Id).ToList();

        // Get all inquiries
        var allInquiries = await _uow.Inquiries.GetAllAsync();
        var myInquiries = allInquiries
            .Where(i => myProperties.Any(p => p.Id == i.PropertyId)).ToList();

        // Get all bookings
        var allBookings = await _uow.VisitBookings.GetAllAsync();
        var myBookings = allBookings
            .Where(b => myProperties.Any(p => p.Id == b.PropertyId)).ToList();

        // Inquiries per property
        var inquiriesPerProperty = myProperties.Select(p => new
        {
            propertyTitle = p.Title.Length > 20 ? p.Title.Substring(0, 20) + "..." : p.Title,
            inquiryCount = myInquiries.Count(i => i.PropertyId == p.Id)
        }).ToList();

        // Property type distribution
        var propertyTypeDistribution = myProperties
            .GroupBy(p => p.PropertyType)
            .Select(g => new { type = g.Key, count = g.Count() })
            .ToList();

        // Booking status distribution
        var bookingStats = new
        {
            pending = myBookings.Count(b => b.Status == "Pending"),
            confirmed = myBookings.Count(b => b.Status == "Confirmed"),
            cancelled = myBookings.Count(b => b.Status == "Cancelled")
        };

        // Total portfolio value
        var totalValue = myProperties.Sum(p => p.Price);

        // Monthly inquiries (last 6 months)
        var monthlyInquiries = Enumerable.Range(0, 6)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Select(month => new
            {
                month = month.ToString("MMM yyyy"),
                count = myInquiries.Count(i =>
                    i.CreatedAt.Month == month.Month &&
                    i.CreatedAt.Year == month.Year)
            })
            .Reverse()
            .ToList();

        return Ok(new
        {
            totalProperties = myProperties.Count,
            totalInquiries = myInquiries.Count,
            totalBookings = myBookings.Count,
            totalValue,
            repliedInquiries = myInquiries.Count(i => i.IsReplied),
            pendingInquiries = myInquiries.Count(i => !i.IsReplied),
            inquiriesPerProperty,
            propertyTypeDistribution,
            bookingStats,
            monthlyInquiries
        });
    }
}