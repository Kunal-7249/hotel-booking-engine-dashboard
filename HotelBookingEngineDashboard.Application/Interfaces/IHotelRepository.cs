using HotelBookingEngineDashboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Interfaces
{
    public interface IHotelRepository
    {
        Task<Hotel?> GetByIdAsync(int id);
        Task<(IEnumerable<Hotel> Items, int TotalCount)> GetAllAsync(string? city = null,int page = 1,int pageSize = 6);
    }
}