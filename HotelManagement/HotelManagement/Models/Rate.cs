using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Rate
	{
		[Key]
		public string RateID { get; set; }

		[Required(ErrorMessage ="Username is required")]
		[RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Name can only contain letters.")]
		public string? Username { get; set; }

		[Required(ErrorMessage ="Email is required")]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@gmail\.com",
			ErrorMessage = "Email must be entered in the format ...gmail.com")]
		public string? Email { get; set; }
		public decimal? Point { get; set; }

		public string? Message { get; set; }
		public DateTime DateCreate { get; set; }
	}
}
