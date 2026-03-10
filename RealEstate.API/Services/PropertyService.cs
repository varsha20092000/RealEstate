using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.API.Services;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _uow;

    public PropertyService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<PropertyDto>> GetAllAsync()
    {
        var props = await _uow.Properties.GetAllAsync();
        return props.Select(MapToDto);
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var prop = await _uow.Properties.GetByIdWithImagesAsync(id);
        return prop == null ? null : MapToDto(prop);
    }
    public async Task<IEnumerable<PropertyDto>> GetFeaturedAsync()
    {
        var props = await _uow.Properties.GetFeaturedAsync();
        return props.Select(MapToDto);
    }

    public async Task<IEnumerable<PropertyDto>> SearchAsync(
        string? city, string? propertyType, string? listingType,
        decimal? minPrice, decimal? maxPrice,
        int? bedrooms, int? bathrooms)
    {
        var props = await _uow.Properties.SearchAsync(
            city, propertyType, listingType,
            minPrice, maxPrice, bedrooms, bathrooms);
        return props.Select(MapToDto);
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto, string userId)
    {
        // Get agent profile for this user
        var agent = (await _uow.Agents.GetAllAsync())
            .FirstOrDefault(a => a.UserId == userId);

        var property = new Property
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            AreaSqFt = dto.AreaSqFt,
            Bedrooms = dto.Bedrooms,
            Bathrooms = dto.Bathrooms,
            Address = dto.Address,
            City = dto.City,
            PropertyType = dto.PropertyType,
            ListingType = dto.ListingType,
            IsFeatured = dto.IsFeatured,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            AgentId = agent?.Id,
            Status = "Available"
        };

        await _uow.Properties.AddAsync(property);
        await _uow.SaveChangesAsync();
        return MapToDto(property);
    }

    public async Task UpdateAsync(UpdatePropertyDto dto)
    {
        var property = await _uow.Properties.GetByIdAsync(dto.Id)
            ?? throw new Exception("Property not found");

        property.Title = dto.Title;
        property.Description = dto.Description;
        property.Price = dto.Price;
        property.AreaSqFt = dto.AreaSqFt;
        property.Bedrooms = dto.Bedrooms;
        property.Bathrooms = dto.Bathrooms;
        property.Address = dto.Address;
        property.City = dto.City;
        property.PropertyType = dto.PropertyType;
        property.ListingType = dto.ListingType;
        property.Status = dto.Status;
        property.IsFeatured = dto.IsFeatured;
        property.Latitude = dto.Latitude;
        property.Longitude = dto.Longitude;
        property.AgentId = dto.AgentId;
        await _uow.Properties.UpdateAsync(property);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _uow.Properties.DeleteAsync(id);
        await _uow.SaveChangesAsync();
    }

    private static PropertyDto MapToDto(Property p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Price = p.Price,
        AreaSqFt = p.AreaSqFt,
        Bedrooms = p.Bedrooms,
        Bathrooms = p.Bathrooms,
        Address = p.Address,
        City = p.City,
        PropertyType = p.PropertyType,
        ListingType = p.ListingType,
        Status = p.Status,
        IsFeatured = p.IsFeatured,
        ViewCount = p.ViewCount,
        CreatedAt = p.CreatedAt,
        ImageUrls = p.Images.Select(i => i.ImageUrl).ToList(),
        Latitude = p.Latitude,
        Longitude = p.Longitude,
        AgentId = p.AgentId,
    };
}