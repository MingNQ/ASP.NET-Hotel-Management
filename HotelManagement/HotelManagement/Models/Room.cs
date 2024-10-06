using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HotelManagement.Models
{
	public class Room
	{
		[Key]
		public int RoomID { get; set; }

		[Required]
		public int FloorID { get; set; }

		[Required]
		public int CategoryID { get; set; }

		public int? ImageID { get; set; }
		public int? ServiceID { get; set; }
		public string RoomNumber { get; set; }

		public string Status { get; set; }
		public string Note { get; set; }

		// Navigation properties
		public Floor Floor { get; set; }
		public Category Category { get; set; }
		public Image Image { get; set; }
		public ICollection<Service> Services { get; set; }
		public ICollection<Booking> Bookings { get; set; }
	}
}
