using global::HotelBookingEngineDashboard.Application.DTOs;
using HotelBookingEngineDashboard.Application.DTOs.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync(string? city = null);
        Task<HotelDetailDto?> GetHotelByIdAsync(int id);
    }
}
