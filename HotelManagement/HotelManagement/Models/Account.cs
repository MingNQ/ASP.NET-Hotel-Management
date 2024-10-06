using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Account
	{
		[Key]
		public int AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string Username { get; set; }

		[Required]
		[MaxLength(50)]
		public string Password { get; set; }

		public string Type { get; set; }

		// Navigation properties
		public ICollection<Staff> Staffs { get; set; }
		public ICollection<Customer> Customers { get; set; }
	}
}
