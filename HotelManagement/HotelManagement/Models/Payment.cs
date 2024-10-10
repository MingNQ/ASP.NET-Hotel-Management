using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Payment
	{
		[Key]
		public string PaymentID { get; set; }

		public string? InvoiceID { get; set; }

		public string? PaymentMethod { get; set; }
		public DateTime DatePayment { get; set; }
		public string? Status { get; set; }

		// Navigation properties
		public virtual Invoice Invoice { get; set; } = null!;
	}
}
