using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Payment
	{
		[Key]
		public int PaymentID { get; set; }

		[Required]
		public int InvoiceID { get; set; }

		public string PaymentMethod { get; set; }
		public decimal Amount { get; set; }
		public DateTime DatePayment { get; set; }
		public string Status { get; set; }

		// Navigation properties
		public Invoice Invoice { get; set; }
	}
}
