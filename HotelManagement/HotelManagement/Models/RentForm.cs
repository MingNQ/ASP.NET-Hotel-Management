using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class RentForm
	{
		[Key]
		public string RentFormID { get; set; }

		public string? BookingID { get; set; }

		public string? RoomID { get; set; }

		public string? StaffID { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCreate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCheckIn { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DateCheckOut { get; set; }
		public decimal Sale { get; set; }

		// Navigation properties
		public virtual Booking Booking { get; set; } = null!;
		public virtual Room Room { get; set; } = null!;
		public virtual Staff? Staff { get; set; }
	}
}
