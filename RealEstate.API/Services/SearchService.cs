using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.API.Services;

public class SearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "properties";

    public SearchService()
    {
        var settings = new ElasticsearchClientSettings(
            new Uri("http://localhost:9200"))
            .DefaultIndex(IndexName);
        _client = new ElasticsearchClient(settings);
    }

    public async Task IndexPropertyAsync(Property property)
    {
        await _client.IndexAsync(new
        {
            id = property.Id,
            title = property.Title,
            description = property.Description,
            city = property.City,
            address = property.Address,
            propertyType = property.PropertyType,
            listingType = property.ListingType,
            price = property.Price,
            bedrooms = property.Bedrooms,
            bathrooms = property.Bathrooms,
            areaSqFt = property.AreaSqFt,
            status = property.Status,
            isFeatured = property.IsFeatured,
            latitude = property.Latitude,
            longitude = property.Longitude,
            createdAt = property.CreatedAt
        }, idx => idx.Index(IndexName).Id(property.Id.ToString()));
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(string query)
    {
        var q = query.ToLower().Trim();

        var response = await _client.SearchAsync<dynamic>(s => s
            .Indices(IndexName)
            .Query(qr => qr
                .QueryString(qs => qs
                    .Query($"*{q}*")
                    .Fields(new[]
                    {
                    "title^3",
                    "city^2",
                    "description",
                    "address",
                    "propertyType",
                    "listingType"
                    })
                )
            )
            .Size(20)
        );

        if (!response.IsValidResponse) return Enumerable.Empty<Property>();

        var properties = response.Documents.Select(doc =>
        {
            try
            {
                return new Property
                {
                    Id = Guid.Parse(doc.GetProperty("id").GetString()),
                    Title = doc.GetProperty("title").GetString() ?? "",
                    Description = doc.GetProperty("description").GetString() ?? "",
                    City = doc.GetProperty("city").GetString() ?? "",
                    Address = doc.GetProperty("address").GetString() ?? "",
                    PropertyType = doc.GetProperty("propertyType").GetString() ?? "",
                    ListingType = doc.GetProperty("listingType").GetString() ?? "",
                    Price = doc.GetProperty("price").GetDecimal(),
                    Bedrooms = doc.GetProperty("bedrooms").GetInt32(),
                    Bathrooms = doc.GetProperty("bathrooms").GetInt32(),
                    AreaSqFt = doc.GetProperty("areaSqFt").GetDecimal(),
                    Status = doc.GetProperty("status").GetString() ?? "",
                    IsFeatured = doc.GetProperty("isFeatured").GetBoolean(),
                };
            }
            catch { return null; }
        }).Where(p => p != null).Cast<Property>();

        return properties;
    }
    public async Task DeletePropertyAsync(Guid propertyId)
    {
        await _client.DeleteAsync(IndexName, propertyId.ToString());
    }

    public async Task ReIndexAllPropertiesAsync(IEnumerable<Property> properties)
    {
        foreach (var property in properties)
        {
            await IndexPropertyAsync(property);
        }
    }
}