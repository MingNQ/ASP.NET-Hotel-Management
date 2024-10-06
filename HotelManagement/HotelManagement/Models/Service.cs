using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Service
	{
		[Key]
		public int ServiceID { get; set; }
		public string ServiceName { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }

		public ICollection<Room> Rooms { get; set; }
	}
}
