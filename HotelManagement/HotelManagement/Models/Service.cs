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

		[Required(ErrorMessage ="Service Name is required")]
		public string? ServiceName { get; set; }

		[Required(ErrorMessage ="Price is required")]
		public decimal Price { get; set; }

		public virtual ICollection<RoomService> RoomServices { get; set; }
	}
}
