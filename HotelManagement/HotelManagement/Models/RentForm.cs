using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class RentForm
	{
		[Key]
		public string RentFormID { get; set; }

		public string? BookingID { get; set; }

		public string? RoomID { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCreate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCheckIn { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCheckOut { get; set; }

		[DisplayFormat(DataFormatString = "{0:p2}", ApplyFormatInEditMode = true)]
		public decimal Sale { get; set; }

		// Navigation properties
		public virtual Booking Booking { get; set; } = null!;
		public virtual Room Room { get; set; } = null!;
	}
}
