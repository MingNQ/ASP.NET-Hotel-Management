using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class RentForm
	{
		[Key]
		public int RentFormID { get; set; }

		[Required]
		public int BookingID { get; set; }

		public DateTime DateCreate { get; set; }
		public DateTime DateCheckOut { get; set; }
		public decimal Sale { get; set; }

		// Navigation properties
		public Booking Booking { get; set; }
	}
}
