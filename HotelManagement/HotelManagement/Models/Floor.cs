using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Floor
	{
		[Key]
		public int FloorID { get; set; }
		public string FloorNumber { get; set; }
		public ICollection<Room> Rooms { get; set; }
	}
}
