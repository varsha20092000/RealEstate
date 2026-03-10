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
public class VisitBookingsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationService _notificationService;

    public VisitBookingsController(IUnitOfWork uow, IEmailService emailService,
        UserManager<AppUser> userManager, INotificationService notificationService)
    {
        _uow = uow;
        _emailService = emailService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    // GET api/visitbookings
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _uow.VisitBookings.GetAllAsync();
        return Ok(bookings);
    }

    // GET api/visitbookings/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var booking = await _uow.VisitBookings.GetByIdAsync(id);
        if (booking == null) return NotFound("Booking not found");
        return Ok(booking);
    }

    // POST api/visitbookings
    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VisitBooking booking)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        booking.BuyerId = userId;
        booking.Status = "Pending";
        booking.CreatedAt = DateTime.UtcNow;

        await _uow.VisitBookings.AddAsync(booking);
        await _uow.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = booking.Id }, booking);
    }

    // PUT api/visitbookings/{id}/confirm
    [Authorize(Roles = "Agent")]
    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var booking = await _uow.VisitBookings.GetByIdAsync(id);
        if (booking == null) return NotFound("Booking not found");

        booking.Status = "Confirmed";
        await _uow.VisitBookings.UpdateAsync(booking);
        await _uow.SaveChangesAsync();

        // Send confirmation email to buyer
        try
        {
            var property = await _uow.Properties.GetByIdAsync(booking.PropertyId);
            var buyer = await _userManager.FindByIdAsync(booking.BuyerId);

            if (property != null && buyer != null)
            {
                await _emailService.SendVisitConfirmationAsync(
                    buyer.Email!,
                    buyer.FullName,
                    property.Title,
                    booking.VisitDate,
                    booking.Notes ?? "");
            }
        }
        catch { /* Don't fail if email fails */ }

        // Send real-time notification to buyer
        try
        {
            var property = await _uow.Properties.GetByIdAsync(booking.PropertyId);
            await _notificationService.SendNotificationToUserAsync(
                booking.BuyerId,
                "Visit Confirmed! 🎉",
                $"Your visit to {property?.Title} on {booking.VisitDate:MMM dd} is confirmed!",
                "booking");
        }
        catch { /* Don't fail if notification fails */ }

        return Ok(booking);
    }

    // PUT api/visitbookings/{id}/cancel
    [Authorize]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var booking = await _uow.VisitBookings.GetByIdAsync(id);
        if (booking == null) return NotFound("Booking not found");

        booking.Status = "Cancelled";
        await _uow.VisitBookings.UpdateAsync(booking);
        await _uow.SaveChangesAsync();

        return Ok(booking);
    }
}