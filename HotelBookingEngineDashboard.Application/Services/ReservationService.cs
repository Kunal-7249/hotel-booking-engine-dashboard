using HotelBookingEngineDashboard.Application.DTOs.Common;
using HotelBookingEngineDashboard.Application.DTOs.Reservations;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Domain.Entities;
using HotelBookingEngineDashboard.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IHotelRepository _hotelRepo;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IReservationRepository reservationRepo,
            IHotelRepository hotelRepo,
            ILogger<ReservationService> logger)
        {
            _reservationRepo = reservationRepo;
            _hotelRepo = hotelRepo;
            _logger = logger;
        }

        public async Task<ReservationDto?> GetByIdAsync(int id)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Reservation", id); 

            return MapToDto(reservation);
        }

        public async Task<PagedResult<ReservationDto>> GetAllAsync(int page = 1,int pageSize = 10)
        {
            var (reservations, totalCount) = await _reservationRepo.GetAllAsync(page, pageSize);

            return new PagedResult<ReservationDto>
            {
                Items = reservations.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ReservationDto>> GetByUserIdAsync(int userId,int page = 1,int pageSize = 10)
        {
            var (reservations, totalCount) = await _reservationRepo.GetByUserIdAsync(userId, page, pageSize);

            return new PagedResult<ReservationDto>
            {
                Items = reservations.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<(bool success, string message, ReservationDto? data)> CreateAsync(CreateReservationDto dto)
        {
            _logger.LogInformation("Creating reservation. HotelId: {HotelId}, GuestName: {GuestName}",
                dto.HotelId, dto.GuestName);

            var hotel = await _hotelRepo.GetByIdAsync(dto.HotelId);
            if (hotel is null)
            {
                _logger.LogWarning("Hotel not found during reservation. HotelId: {HotelId}", dto.HotelId);
                throw new NotFoundException("Hotel", dto.HotelId);
            }

            if (dto.GuestAge < 18)
                throw new BadRequestException("Guest must be at least 18 years old.");

            if (dto.NumberOfGuests < 1)
                throw new BadRequestException("Number of guests must be at least 1.");

            if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new BadRequestException("Check-in date must be today or a future date.");

            if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
                throw new BadRequestException("Check-out date must be after check-in date.");

            var reservation = new Reservation
            {
                BookingRef = $"BK-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",  
                HotelId = dto.HotelId,
                UserId = dto.UserId,
                GuestName = dto.GuestName,
                GuestAge = dto.GuestAge,
                NumberOfGuests = dto.NumberOfGuests,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                Status = ReservationStatus.CONFIRMED,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _reservationRepo.CreateAsync(reservation); 

            _logger.LogInformation("Reservation created. BookingRef: {BookingRef}, UserId: {UserId}",
                created.BookingRef, dto.UserId);

            var full = await _reservationRepo.GetByIdAsync(created.ReservationId);
            return (true, "Reservation created successfully.", MapToDto(full!));
        }

        public async Task<(bool success, string message)> UpdateAsync(int id, UpdateReservationDto dto)
        {
            _logger.LogInformation("Updating reservation. ReservationId: {ReservationId}", id);

            var reservation = await _reservationRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Reservation", id); 

            if (reservation.Status == ReservationStatus.CANCELLED)
                throw new BadRequestException("Cancelled reservations cannot be modified.");

            if (dto.GuestAge < 18)
                throw new BadRequestException("Guest must be at least 18 years old.");

            if (dto.NumberOfGuests < 1)
                throw new BadRequestException("Number of guests must be at least 1.");

            if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new BadRequestException("Check-in date must be today or a future date.");

            if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
                throw new BadRequestException("Check-out date must be after check-in date.");

            reservation.GuestName = dto.GuestName;
            reservation.GuestAge = dto.GuestAge;
            reservation.NumberOfGuests = dto.NumberOfGuests;
            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;

            await _reservationRepo.UpdateAsync(reservation);

            _logger.LogInformation("Reservation updated. BookingRef: {BookingRef}", reservation.BookingRef);

            return (true, "Reservation updated successfully.");
        }

        public async Task<(bool success, string message)> CancelAsync(int id)
        {
            _logger.LogInformation("Cancelling reservation. ReservationId: {ReservationId}", id);

            var reservation = await _reservationRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Reservation", id); 

            if (reservation.Status == ReservationStatus.CANCELLED)
                throw new BadRequestException("Reservation is already cancelled.");

            reservation.Status = ReservationStatus.CANCELLED;
            await _reservationRepo.UpdateAsync(reservation);

            _logger.LogInformation("Reservation cancelled. BookingRef: {BookingRef}", reservation.BookingRef);

            return (true, "Reservation cancelled successfully.");
        }

        // Private mapper 
        private static ReservationDto MapToDto(Reservation r) => new()
        {
            ReservationId = r.ReservationId,
            BookingRef = r.BookingRef,
            HotelName = r.Hotel?.Name ?? string.Empty,
            GuestName = r.GuestName,
            GuestAge = r.GuestAge,              
            NumberOfGuests = r.NumberOfGuests,  
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            Status = r.Status.ToString()
        };
    }
}