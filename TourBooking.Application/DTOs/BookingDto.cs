using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBooking.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public int TourId { get; set; }
        public string? TourTitle { get; set; } // optional
    }
}
