using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Image
	{
		[Key]
		public int ImageID { get; set; }
		public string ImageUrl { get; set; }

		public ICollection<Room> Rooms { get; set; }
	}
}
