using HotelManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Staff
	{
		public Staff()
		{
			Invoices = new HashSet<Invoice>();
			RentForms = new HashSet<RentForm>();
        }

		[Key]
		public string StaffID { get; set; }

		public int? AccountID { get; set; }

		[Required(ErrorMessage ="First name is required")]
		[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name can only contain letters.")]
		[MaxLength(50)]
		public string? FirstName { get; set; }

        [Required(ErrorMessage ="Last name is not null!")]
		[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name can only contain letters.")]
        [MaxLength(50)]
		public string? LastName { get; set; }

        [Required]
        public Gender Gender { get; set; }

		[Required(ErrorMessage ="Address is required")]
		public string? Address { get; set; }

		[Required]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@gmail\.com",
			ErrorMessage = "Email must be entered in the format ...gmail.com")]
		public string? Email { get; set; }

		[Required(ErrorMessage ="Phone is not null!")]
		[RegularExpression(@"^\d+$", ErrorMessage = "Phone number can only contain digits.")]
		[StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 digits.")]
		public string? Phone { get; set; }

        [Required(ErrorMessage ="Role must be selected")]
        public Role Role { get; set; }

		// Navigation properties
		public virtual Account? Account { get; set; }
		public virtual ICollection<Invoice> Invoices { get; set; }
		public virtual ICollection<RentForm> RentForms { get; set; }
	}
}
