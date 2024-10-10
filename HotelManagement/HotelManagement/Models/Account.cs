using HotelManagement.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Account
	{
		public Account()
		{
			Staffs = new HashSet<Staff>();
			Customers = new HashSet<Customer>();
		}
		public int AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string? Username { get; set; }

		[Required]
		[MaxLength(50)]
		public string? Password { get; set; }

		public AccountType Type { get; set; }

		// Navigation properties
		public virtual ICollection<Staff> Staffs { get; set; }
		public virtual ICollection<Customer> Customers { get; set; }
	}
}
