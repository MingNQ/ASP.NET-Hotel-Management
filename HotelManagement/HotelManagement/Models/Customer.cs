using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Customer
	{
		[Key]
		public int CustomerID { get; set; }

		[Required]
		public int AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[MaxLength(50)]
		public string LastName { get; set; }

		public string Gender { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		public string Phone { get; set; }

		public string Address { get; set; }

		public string Membership { get; set; }

		// Navigation properties
		public Account Account { get; set; }
		public ICollection<Booking> Bookings { get; set; }
	}
}
