using HotelManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Customer
	{
		public Customer() 
		{
			Bookings = new HashSet<Booking>();
		}

		[Key]
		public string? CustomerID { get; set; }

		public int? AccountID { get; set; }

		[Required(ErrorMessage ="First is not Null")]
		[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name can only contain letters.")]
		[MaxLength(50)]
		public string? FirstName { get; set; }

		[Required(ErrorMessage = "Name is not Null")]
		[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name can only contain letters.")]
        [MaxLength(50)]
		public string? LastName { get; set; }

		public Gender Gender { get; set; }

		//[EmailAddress]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@gmail\.com",
			ErrorMessage = "Email must be entered in the format ...gmail.com")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Phone is not null!")]
		[RegularExpression(@"^\d+$", ErrorMessage = "Phone number can only contain digits.")]
		[StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 digits.")]
		public string? Phone { get; set; }

		public string? Address { get; set; }

		public Membership Membership { get; set; }

		// Navigation properties
		public virtual Account? Account { get; set; }		
		public ICollection<Booking> Bookings { get; set; }
	}
}
