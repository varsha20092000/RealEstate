using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces;

public interface IPropertyRepository : IGenericRepository<Property>
{
    Task<IEnumerable<Property>> GetFeaturedAsync();
    Task<Property?> GetByIdWithImagesAsync(Guid id);
    Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId);
    Task<IEnumerable<Property>> SearchAsync(
        string? city,
        string? propertyType,
        string? listingType,
        decimal? minPrice,
        decimal? maxPrice,
        int? bedrooms,
        int? bathrooms);
}