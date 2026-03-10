using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using System.Security.Claims;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public FavoritesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET api/favorites
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var favorites = await _uow.Favorites.GetAllAsync();
        return Ok(favorites);
    }

    // POST api/favorites
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Favorite favorite)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        favorite.UserId = userId;
        favorite.SavedAt = DateTime.UtcNow;

        await _uow.Favorites.AddAsync(favorite);
        await _uow.SaveChangesAsync();
        return Ok(favorite);
    }

    // DELETE api/favorites/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var exists = await _uow.Favorites.ExistsAsync(id);
        if (!exists) return NotFound("Favorite not found");

        await _uow.Favorites.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}