using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Image
	{
		[Key]
		public string ImageID { get; set; }
		[Required]
		public string? RoomID {  get; set; }
		public string? ImageUrl { get; set; }

		public virtual Room Room { get; set; } = null!;
	}
}
