using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IPropertyRepository Properties { get; }
    public IGenericRepository<Agent> Agents { get; }
    public IGenericRepository<Inquiry> Inquiries { get; }
    public IGenericRepository<Favorite> Favorites { get; }
    public IGenericRepository<Review> Reviews { get; }
    public IGenericRepository<VisitBooking> VisitBookings { get; }
    public IGenericRepository<PropertyImage> PropertyImages { get; }
    public IGenericRepository<Payment> Payments { get; }
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Properties = new PropertyRepository(context);
        Agents = new GenericRepository<Agent>(context);
        Inquiries = new GenericRepository<Inquiry>(context);
        Favorites = new GenericRepository<Favorite>(context);
        Reviews = new GenericRepository<Review>(context);
        VisitBookings = new GenericRepository<VisitBooking>(context);
        PropertyImages = new GenericRepository<PropertyImage>(context);
        Payments = new GenericRepository<Payment>(context);
    }
    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}