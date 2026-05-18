using global::HotelBookingEngineDashboard.Application.Interfaces;
using global::HotelBookingEngineDashboard.Domain.Entities;
using global::HotelBookingEngineDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace HotelBookingEngineDashboard.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync(string? city = null)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(h => h.City.ToLower() == city.ToLower());

            return await query.ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == id);
        }
    }
}
