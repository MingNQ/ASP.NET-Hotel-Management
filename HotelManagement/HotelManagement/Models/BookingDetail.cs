using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotelManagement.Models
{
    public class BookingDetail
    {
        [Key]
        [ForeignKey("Booking")]
        public string BookingID { get; set; }
        [Required]
        public string? CategoryID { get; set; }
        public int NumberRoom { get; set; }

        // Navigation Properties
        public virtual Category? Category { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
