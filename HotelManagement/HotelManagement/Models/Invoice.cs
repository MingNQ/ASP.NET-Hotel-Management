using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Invoice
	{
		[Key]
		public string InvoiceID { get; set; }

		public string? StaffID { get; set; }

		public string? BookingID { get; set; }

		public DateTime DateCreate { get; set; }
		public string? Note { get; set; }

		// Navigation properties
		public virtual Staff Staff { get; set; } = null!;
		public virtual Booking Booking { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }
}
