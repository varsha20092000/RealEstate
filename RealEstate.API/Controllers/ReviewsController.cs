using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using System.Security.Claims;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ReviewsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET api/reviews
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _uow.Reviews.GetAllAsync();
        return Ok(reviews);
    }

    // GET api/reviews/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _uow.Reviews.GetByIdAsync(id);
        if (review == null) return NotFound("Review not found");
        return Ok(review);
    }

    // POST api/reviews
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Review review)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        if (review.Rating < 1 || review.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        review.ReviewerId = userId;
        review.CreatedAt = DateTime.UtcNow;

        await _uow.Reviews.AddAsync(review);
        await _uow.SaveChangesAsync();
        return Ok(review);
    }

    // DELETE api/reviews/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var exists = await _uow.Reviews.ExistsAsync(id);
        if (!exists) return NotFound("Review not found");

        await _uow.Reviews.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}