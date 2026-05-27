using global::HotelBookingEngineDashboard.Application.DTOs;
using HotelBookingEngineDashboard.Application.DTOs.Common;
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
        Task<HotelDetailDto?> GetHotelByIdAsync(int id);
        Task<PagedResult<HotelDto>> GetAllHotelsAsync(string? city = null,int page = 1,int pageSize = 6);
    }
}
