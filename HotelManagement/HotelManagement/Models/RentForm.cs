using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class RentForm
	{
		[Key]
		public string RentFormID { get; set; }

		public string? BookingID { get; set; }

		public string? RoomID { get; set; }

		public DateTime DateCreate { get; set; }
		public DateTime DateCheckIn { get; set; }
		public DateTime DateCheckOut { get; set; }
		public decimal Sale { get; set; }

		// Navigation properties
		public virtual Booking Booking { get; set; } = null!;
		public virtual Room Room { get; set; } = null!;
	}
}
