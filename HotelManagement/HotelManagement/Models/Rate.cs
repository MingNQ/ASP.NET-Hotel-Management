using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Rate
	{
		[Key]
		public string RateID { get; set; }
		public decimal Point { get; set; }

		public string? Message { get; set; }
		public DateTime DateCreate { get; set; }

	}
}
