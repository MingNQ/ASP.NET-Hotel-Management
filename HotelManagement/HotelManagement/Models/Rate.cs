using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Rate
	{
		[Key]
		public string RateID { get; set; }

		public string? CustomerID { get; set; }
		public decimal Point { get; set; }

		public string? Message { get; set; }
		public DateTime DateCreate { get; set; }

		// Navigation properties
		public virtual Customer Customer { get; set; } = null!;
	}
}
