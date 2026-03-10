using RealEstate.Application.DTOs;

namespace RealEstate.Application.Interfaces;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllAsync();
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PropertyDto>> GetFeaturedAsync();
    Task<IEnumerable<PropertyDto>> SearchAsync(
        string? city, string? propertyType, string? listingType,
        decimal? minPrice, decimal? maxPrice,
        int? bedrooms, int? bathrooms);
    Task<PropertyDto> CreateAsync(CreatePropertyDto dto, string userId);
    Task UpdateAsync(UpdatePropertyDto dto);
    Task DeleteAsync(Guid id);
}