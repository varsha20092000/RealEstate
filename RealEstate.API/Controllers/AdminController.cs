using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<AppUser> _userManager;

    public AdminController(IUnitOfWork uow, UserManager<AppUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    // GET api/admin/stats
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var properties = await _uow.Properties.GetAllAsync();
        var agents = await _uow.Agents.GetAllAsync();
        var inquiries = await _uow.Inquiries.GetAllAsync();
        var bookings = await _uow.VisitBookings.GetAllAsync();
        var reviews = await _uow.Reviews.GetAllAsync();
        var users = _userManager.Users.ToList();

        return Ok(new
        {
            TotalProperties = properties.Count(),
            TotalAgents = agents.Count(),
            TotalInquiries = inquiries.Count(),
            TotalBookings = bookings.Count(),
            TotalReviews = reviews.Count(),
            TotalUsers = users.Count,
            FeaturedProperties = properties.Count(p => p.IsFeatured),
            PendingBookings = bookings.Count(b => b.Status == "Pending"),
        });
    }

    // GET api/admin/users
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.IsActive,
                u.CreatedAt
            }).ToList();
        return Ok(users);
    }

    // PUT api/admin/agents/{id}/approve
    [HttpPut("agents/{id}/approve")]
    public async Task<IActionResult> ApproveAgent(Guid id)
    {
        var agent = await _uow.Agents.GetByIdAsync(id);
        if (agent == null) return NotFound("Agent not found");

        agent.IsApproved = true;
        await _uow.Agents.UpdateAsync(agent);
        await _uow.SaveChangesAsync();
        return Ok(new { message = "Agent approved successfully!" });
    }

    // PUT api/admin/reviews/{id}/approve
    [HttpPut("reviews/{id}/approve")]
    public async Task<IActionResult> ApproveReview(Guid id)
    {
        var review = await _uow.Reviews.GetByIdAsync(id);
        if (review == null) return NotFound("Review not found");

        review.IsApproved = true;
        await _uow.Reviews.UpdateAsync(review);
        await _uow.SaveChangesAsync();
        return Ok(new { message = "Review approved successfully!" });
    }

    // DELETE api/admin/properties/{id}
    [HttpDelete("properties/{id}")]
    public async Task<IActionResult> DeleteProperty(Guid id)
    {
        var exists = await _uow.Properties.ExistsAsync(id);
        if (!exists) return NotFound("Property not found");

        await _uow.Properties.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Ok(new { message = "Property deleted successfully!" });
    }
}