using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Category
	{
		public Category() 
		{
			Rooms = new HashSet<Room>();
		}

		[Key]
		public string CategoryID { get; set; }

		[Required(ErrorMessage ="Type name is required")]
		[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Type Name can only contain letters")]
		public string? TypeName { get; set; }
		public decimal Capacity { get; set; }
		public decimal Price { get; set; }

		public virtual ICollection<Room> Rooms { get; set; }
	}
}
