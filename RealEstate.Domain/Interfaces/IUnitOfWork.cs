using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    IGenericRepository<Agent> Agents { get; }
    IGenericRepository<Inquiry> Inquiries { get; }
    IGenericRepository<Favorite> Favorites { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<VisitBooking> VisitBookings { get; }
    IGenericRepository<RealEstate.Domain.Entities.PropertyImage> PropertyImages { get; }
    IGenericRepository<Payment> Payments { get; }
    Task<int> SaveChangesAsync();
}