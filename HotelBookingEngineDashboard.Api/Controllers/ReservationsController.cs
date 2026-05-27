using HotelBookingEngineDashboard.Application.DTOs.Reservations;
using HotelBookingEngineDashboard.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelBookingEngineDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _reservationService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation is null)
                return NotFound(new { message = "Reservation not found." });
            return Ok(reservation);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ExternalUser")]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.UserId = GetCurrentUserId();

            var (success, message, data) = await _reservationService.CreateAsync(dto);
            return Ok(new { message, data });
        }

        [HttpGet("my-bookings")]
        [Authorize(Roles = "Admin,ExternalUser")]
        public async Task<IActionResult> GetMyBookings([FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _reservationService.GetByUserIdAsync(
                GetCurrentUserId(), page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
        {
            var (success, message) = await _reservationService.UpdateAsync(id, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var (success, message) = await _reservationService.CancelAsync(id);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

    }
}
