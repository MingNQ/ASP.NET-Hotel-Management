using HotelManagement.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{

	public class Account
	{
		public Account()
		{
			Customers = new HashSet<Customer>();
		}
		public int AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string? Username { get; set; }

		[Required]
		public string? Password { get; set; }

		public AccountType Type { get; set; }
		public bool Active { get; set; }

		// Navigation properties
		public virtual Staff? Staff { get; set; }
		public virtual ICollection<Customer> Customers { get; set; }
	}
}
