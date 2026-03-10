using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Interfaces;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IPropertyService _propertyService;
    private readonly ISearchService _searchService;

    public PropertiesController(IUnitOfWork uow, IPropertyService propertyService,
        ISearchService searchService)
    {
        _uow = uow;
        _propertyService = propertyService;
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _propertyService.GetAllAsync());

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured()
        => Ok(await _propertyService.GetFeaturedAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await _propertyService.GetByIdAsync(id);
        return property == null ? NotFound() : Ok(property);
    }

    [Authorize(Roles = "Agent,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var result = await _propertyService.CreateAsync(dto, userId);
        await _searchService.IndexPropertyAsync(await _uow.Properties.GetByIdAsync(result.Id)
            ?? new RealEstate.Domain.Entities.Property());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _propertyService.UpdateAsync(dto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _propertyService.DeleteAsync(id);
        await _searchService.DeletePropertyAsync(id);
        return NoContent();
    }

    // GET api/properties/search — SQL filter search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? city,
        [FromQuery] string? propertyType,
        [FromQuery] string? listingType,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? bedrooms,
        [FromQuery] int? bathrooms,
        [FromQuery] bool? isFeatured,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _propertyService.SearchAsync(
            city, propertyType, listingType,
            minPrice, maxPrice, bedrooms, bathrooms);

        if (isFeatured.HasValue)
            result = result.Where(p => p.IsFeatured == isFeatured.Value);

        result = sortBy switch
        {
            "price_asc" => result.OrderBy(p => p.Price),
            "price_desc" => result.OrderByDescending(p => p.Price),
            "newest" => result.OrderByDescending(p => p.CreatedAt),
            _ => result.OrderByDescending(p => p.CreatedAt)
        };

        var total = result.Count();
        var paged = result.Skip((page - 1) * pageSize).Take(pageSize);

        return Ok(new
        {
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Data = paged
        });
    }

    // GET api/properties/fulltext?q=luxury mumbai — Elasticsearch full-text search
    [HttpGet("fulltext")]
    public async Task<IActionResult> FullTextSearch([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query is required");

        var results = await _searchService.SearchPropertiesAsync(q);
        return Ok(results);
    }

    // POST api/properties/reindex — reindex all properties
    [Authorize(Roles = "Admin")]
    [HttpPost("reindex")]
    public async Task<IActionResult> ReIndex()
    {
        var properties = await _uow.Properties.GetAllAsync();
        await _searchService.ReIndexAllPropertiesAsync(properties);
        return Ok(new { message = $"Indexed {properties.Count()} properties" });
    }

    // POST api/properties/{id}/images
    [Authorize]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var property = await _propertyService.GetByIdAsync(id);
        if (property == null) return NotFound();

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"https://localhost:7056/images/{fileName}";

        var image = new RealEstate.Domain.Entities.PropertyImage
        {
            PropertyId = id,
            ImageUrl = imageUrl
        };

        await _uow.PropertyImages.AddAsync(image);
        await _uow.SaveChangesAsync();

        return Ok(new { imageUrl });
    }
}