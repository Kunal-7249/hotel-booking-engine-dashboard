using HotelBookingEngineDashboard.Application.DTOs.Common;
using HotelBookingEngineDashboard.Application.DTOs.Reservations;
using HotelBookingEngineDashboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Services
{
    public interface IReservationService
    {
        Task<ReservationDto?> GetByIdAsync(int id);
        Task<PagedResult<ReservationDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<PagedResult<ReservationDto>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10);
        Task<(bool success, string message, ReservationDto? data)> CreateAsync(CreateReservationDto dto);
        Task<(bool success, string message)> UpdateAsync(int id, UpdateReservationDto dto);
        Task<(bool success, string message)> CancelAsync(int id);
    }
}