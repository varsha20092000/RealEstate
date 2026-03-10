using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces;

public interface ISearchService
{
    Task IndexPropertyAsync(Property property);
    Task<IEnumerable<Property>> SearchPropertiesAsync(string query);
    Task DeletePropertyAsync(Guid propertyId);
    Task ReIndexAllPropertiesAsync(IEnumerable<Property> properties);
}