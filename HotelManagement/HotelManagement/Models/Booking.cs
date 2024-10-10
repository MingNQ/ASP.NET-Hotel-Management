using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Booking
	{
		[Key]
		public string BookingID { get; set; }

		public string? CustomerID { get; set; }

		public DateTime DateCome { get; set; }
		public DateTime DateGo { get; set; }
		public decimal Deposit { get; set; }

		// Navigation properties
		public virtual Customer Customer { get; set; } = null!;
		public virtual RentForm RentForm { get; set; } = null!;
	}
}
