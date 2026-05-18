using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Domain.Entities
{
    public class Hotel
    {
        public int HotelId {  get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int StarRating { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description   { get; set; }   
        public bool IsAvailable { get; set; } = true;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

