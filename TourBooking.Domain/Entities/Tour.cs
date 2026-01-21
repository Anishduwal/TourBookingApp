using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBooking.Domain.Entities
{
    public class Tour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationInHours { get; set; }
        public bool IsActive { get; set; }
    }
}
