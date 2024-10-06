using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Invoice
	{
		[Key]
		public int InvoiceID { get; set; }

		[Required]
		public int StaffID { get; set; }

		[Required]
		public int BookingID { get; set; }

		public DateTime DateCreate { get; set; }
		public string Note { get; set; }

		// Navigation properties
		public Staff Staff { get; set; }
		public Booking Booking { get; set; }
		public ICollection<Payment> Payments { get; set; }
	}
}
