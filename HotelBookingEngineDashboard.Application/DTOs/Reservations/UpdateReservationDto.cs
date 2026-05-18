using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.DTOs.Reservations
{
    public class UpdateReservationDto
    {
        [Required(ErrorMessage = "Guest name is required.")]
        [MaxLength(200, ErrorMessage = "Guest name cannot exceed 200 characters.")]
        public string GuestName { get; set; } = string.Empty;

        [Range(18, 120, ErrorMessage = "Guest must be at least 18 years old.")]
        public int GuestAge { get; set; }

        [Range(1, 100, ErrorMessage = "Number of guests must be at least 1.")]
        public int NumberOfGuests { get; set; }

        [Required(ErrorMessage = "Check-in date is required.")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        public DateTime CheckOutDate { get; set; }
    }
}