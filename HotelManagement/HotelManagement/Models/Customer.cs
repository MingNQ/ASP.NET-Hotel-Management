using HotelManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Customer
	{
		public Customer() 
		{
			Bookings = new HashSet<Booking>();
			Rates = new HashSet<Rate>();
		}

		[Key]
		public string CustomerID { get; set; }

		public int? AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string? FirstName { get; set; }

		[MaxLength(50)]
		public string? LastName { get; set; }

		public Gender Gender { get; set; }

		[EmailAddress]
		public string? Email { get; set; }

		[Phone]
		public string? Phone { get; set; }

		public string? Address { get; set; }

		public Membership Membership { get; set; }

		// Navigation properties
		public virtual Account Account { get; set; } = null!;
		public ICollection<Booking> Bookings { get; set; }
		public ICollection<Rate> Rates { get; set; }
	}
}
