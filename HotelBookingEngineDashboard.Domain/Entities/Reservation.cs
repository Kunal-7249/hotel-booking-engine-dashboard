using HotelBookingEngineDashboard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Domain.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public string BookingRef { get; set; } = string.Empty;   
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;                
        public string GuestName { get; set; } = string.Empty;
        public int GuestAge { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.CONFIRMED;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }  
        public User? User { get; set; }
    }
}



