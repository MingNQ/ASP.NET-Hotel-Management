using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Category
	{
		[Key]
		public int CategoryID { get; set; }
		public string TypeName { get; set; }
		public int Capacity { get; set; }
		public decimal Price { get; set; }

		public ICollection<Room> Rooms { get; set; }
	}
}
