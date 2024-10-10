using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
	public class Service
	{
		public Service()
		{
			RoomServices = new HashSet<RoomService>();
		}

		[Key]
		public string ServiceID { get; set; }
		public string? ServiceName { get; set; }
		public decimal Price { get; set; }

		public virtual ICollection<RoomService> RoomServices { get; set; }
	}
}
