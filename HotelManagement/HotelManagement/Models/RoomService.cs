using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HotelManagement.Models
{
	public class RoomService
	{
		[Key]
		public int RoomServiceID { get; set; }

		public string? RoomID { get; set; }
		public virtual Room? Room { get; set; }
		public string? ServiceID { get; set; }
		public virtual Service? Service { get; set; }
		
	}
}
