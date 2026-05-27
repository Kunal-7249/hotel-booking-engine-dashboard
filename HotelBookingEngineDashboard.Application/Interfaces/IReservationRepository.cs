using HotelBookingEngineDashboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id);
        Task<(IEnumerable<Reservation> Items, int TotalCount)> GetAllAsync(int page = 1,int pageSize = 10);
        Task<(IEnumerable<Reservation> Items, int TotalCount)> GetByUserIdAsync(int userId,int page = 1,int pageSize = 10);
        Task<int> GetNextReservationIdAsync();
        Task<Reservation> CreateAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
    }
}
