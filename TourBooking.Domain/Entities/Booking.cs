using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBooking.Domain.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = null!;

        [Required]
        public string CustomerEmail { get; set; } = null!;

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public int NumberOfPeople { get; set; }

        // Foreign Key
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour Tour { get; set; } = null!;
    }
}
