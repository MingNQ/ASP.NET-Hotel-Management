using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Rate
	{
		[Key]
		public int RateID { get; set; }

		[Required]
		public int CustomerID { get; set; }

		public string Message { get; set; }
		public DateTime DateCreate { get; set; }

		// Navigation properties
		public Customer Customer { get; set; }
	}
}
