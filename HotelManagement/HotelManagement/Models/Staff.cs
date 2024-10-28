using HotelManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Staff
	{
		public Staff()
		{
			Invoices = new HashSet<Invoice>();
		}

		[Key]
		public string StaffID { get; set; }

		public int? AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		public string? FirstName { get; set; }

		[MaxLength(50)]
		public string? LastName { get; set; }

		public Gender Gender { get; set; }
		public string? Address { get; set; }

		[EmailAddress]
		public string? Email { get; set; }

		[Phone]
		public string? Phone { get; set; }

		public Role Role { get; set; }

		// Navigation properties
		public virtual Account? Account { get; set; }
		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
