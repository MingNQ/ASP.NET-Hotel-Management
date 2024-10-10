using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HotelManagement.Models
{
	public class Room
	{
		public Room()
		{
			RoomServices = new HashSet<RoomService>();
			RentForms = new HashSet<RentForm>();
			Images = new HashSet<Image>();
		}

		[Key]
		public string RoomID { get; set; }

		public string? CategoryID { get; set; }
		public string? Status { get; set; }
		public string? Description { get; set; }

		// Navigation properties
		public virtual Category Category { get; set; } = null!;
		public virtual ICollection<RoomService> RoomServices { get; set; }
		public virtual ICollection<RentForm> RentForms { get; set; }
		public virtual ICollection<Image> Images { get; set; }
	}
}
