using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositories;

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context) { }
    public async Task<Property?> GetByIdWithImagesAsync(Guid id)
    => await _context.Properties
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == id);
    public new async Task<IEnumerable<Property>> GetAllAsync()
      => await _context.Properties
        .Include(p => p.Images)
        .ToListAsync();
    public async Task<IEnumerable<Property>> GetFeaturedAsync()
        => await _context.Properties
            .Where(p => p.IsFeatured)
            .Include(p => p.Images)
            .ToListAsync();

    public async Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId)
        => await _context.Properties
            .Where(p => p.AgentId == agentId)
            .Include(p => p.Images)
            .ToListAsync();

    public async Task<IEnumerable<Property>> SearchAsync(
        string? city,
        string? propertyType,
        string? listingType,
        decimal? minPrice,
        decimal? maxPrice,
        int? bedrooms,
        int? bathrooms)
    {
        var query = _context.Properties
            .Include(p => p.Images)
            .AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(p => p.City.Contains(city));

        if (!string.IsNullOrEmpty(propertyType))
            query = query.Where(p => p.PropertyType == propertyType);

        if (!string.IsNullOrEmpty(listingType))
            query = query.Where(p => p.ListingType == listingType);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (bedrooms.HasValue)
            query = query.Where(p => p.Bedrooms == bedrooms.Value);

        if (bathrooms.HasValue)
            query = query.Where(p => p.Bathrooms == bathrooms.Value);

        return await query.ToListAsync();
    }
}