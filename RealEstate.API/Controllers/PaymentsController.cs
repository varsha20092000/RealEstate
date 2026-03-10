using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.API.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using System.Security.Claims;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;

    public PaymentsController(PaymentService paymentService, IUnitOfWork uow, IEmailService emailService, UserManager<AppUser> userManager)
    {
        _paymentService = paymentService;
        _uow = uow;
        _emailService = emailService;
        _userManager = userManager;
    }

    // POST api/payments/create-order
    [Authorize]
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var booking = await _uow.VisitBookings.GetByIdAsync(dto.BookingId);
        if (booking == null) return NotFound("Booking not found");

        // Deposit amount — 1% of property price or fixed amount
        var depositAmount = dto.Amount;

        var order = _paymentService.CreateOrder(depositAmount);
        return Ok(order);
    }

    // POST api/payments/verify
    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
    {
        var isValid = _paymentService.VerifyPayment(
            dto.RazorpayOrderId,
            dto.RazorpayPaymentId,
            dto.RazorpaySignature);

        if (!isValid) return BadRequest("Payment verification failed!");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        // Save payment record
        var payment = new Payment
        {
            VisitBookingId = dto.BookingId,
            BuyerId = userId,
            Amount = dto.Amount,
            RazorpayOrderId = dto.RazorpayOrderId,
            RazorpayPaymentId = dto.RazorpayPaymentId,
            RazorpaySignature = dto.RazorpaySignature,
            Status = "Paid"
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.SaveChangesAsync();

        // Send payment confirmation email
        try
        {
            var booking = await _uow.VisitBookings.GetByIdAsync(dto.BookingId);
            var user = await _userManager.FindByIdAsync(userId);
            if (booking != null && user != null)
            {
                var property = await _uow.Properties.GetByIdAsync(booking.PropertyId);
                await _emailService.SendPaymentConfirmationAsync(
                    user.Email!,
                    user.FullName,
                    property?.Title ?? "Property",
                    dto.Amount,
                    dto.RazorpayPaymentId
                );
            }
        }
        catch { }

        return Ok(new { message = "Payment successful!", paymentId = payment.Id });
    }

    // GET api/payments/booking/{bookingId}
    [Authorize]
    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetByBooking(Guid bookingId)
    {
        var payments = await _uow.Payments.GetAllAsync();
        var payment = payments.FirstOrDefault(p => p.VisitBookingId == bookingId);
        return payment == null ? NotFound() : Ok(payment);
    }
}

public class CreateOrderDto
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
}

public class VerifyPaymentDto
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public string RazorpaySignature { get; set; } = string.Empty;
}