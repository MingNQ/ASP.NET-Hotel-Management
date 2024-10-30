using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace HotelManagement.Models
{
    [Keyless]
    public class BookingDetail
    {
        [Required]
        public string? BookingID { get; set; }
        [Required]
        public string? CategoryID { get; set; }
        public int NumberRoom { get; set; }

        // Navigation Properties
        public virtual Category? Category { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
