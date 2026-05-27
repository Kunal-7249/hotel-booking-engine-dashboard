using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Domain.Entities;
using HotelBookingEngineDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task<(IEnumerable<Reservation> Items, int TotalCount)> GetAllAsync(int page = 1,int pageSize = 10)
        {
            var query = _context.Reservations
                .Include(r => r.Hotel)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Reservation> Items, int TotalCount)> GetByUserIdAsync(int userId,int page = 1,int pageSize = 10)
        {
            var query = _context.Reservations
                .Include(r => r.Hotel)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> GetNextReservationIdAsync()
        {
            // Used to generate BK-10001, BK-10002 etc.
            var maxId = await _context.Reservations
                .MaxAsync(r => (int?)r.ReservationId) ?? 0;
            return maxId + 1;
        }

        public async Task<Reservation> CreateAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

    }
}
