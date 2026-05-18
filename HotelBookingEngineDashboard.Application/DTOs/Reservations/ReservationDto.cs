using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Application.DTOs.Reservations
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public string BookingRef { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public int GuestAge { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
