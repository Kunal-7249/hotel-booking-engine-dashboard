using HotelBookingEngineDashboard.Application.DTOs.Common;
using HotelBookingEngineDashboard.Application.DTOs.Hotels;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly ILogger<HotelService> _logger;
        public HotelService(IHotelRepository hotelRepository,ILogger<HotelService> logger) {
            _hotelRepository = hotelRepository;
            _logger = logger;
        }

        public async Task<PagedResult<HotelDto>> GetAllHotelsAsync(string? city = null,int page = 1,int pageSize = 6)
        {
            _logger.LogInformation("Fetching hotels. City: {City}, Page: {Page}, PageSize: {PageSize}",
                city ?? "none", page, pageSize);

            var (hotels, totalCount) = await _hotelRepository.GetAllAsync(city, page, pageSize);

            return new PagedResult<HotelDto>
            {
                Items = hotels.Select(h => new HotelDto
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    City = h.City,
                    StarRating = h.StarRating,
                    PricePerNight = h.PricePerNight
                }),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<HotelDetailDto?> GetHotelByIdAsync(int id)
        {
            _logger.LogInformation("Fetching hotel detail. HotelId: {HotelId}", id);

            var hotel = await _hotelRepository.GetByIdAsync(id);

            if (hotel is null)
            {
                _logger.LogWarning("Hotel not found. HotelId: {HotelId}", id);
                throw new NotFoundException("Hotel", id);
            }

            return new HotelDetailDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                City = hotel.City,
                StarRating = hotel.StarRating,
                PricePerNight = hotel.PricePerNight,
                Description = hotel.Description,
                IsAvailable = hotel.IsAvailable
            };
        }
    }
}
