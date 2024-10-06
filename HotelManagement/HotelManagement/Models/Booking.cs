using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Booking
	{
		[Key]
		public int BookingID { get; set; }

		[Required]
		public int CustomerID { get; set; }

		[Required]
		public int RoomID { get; set; }

		public DateTime DateCome { get; set; }
		public DateTime DateGo { get; set; }
		public int Quantity { get; set; }
		public decimal Subtotal { get; set; }

		// Navigation properties
		public Customer Customer { get; set; }
		public Room Room { get; set; }
		public RentForm RentForm { get; set; }
	}
}
