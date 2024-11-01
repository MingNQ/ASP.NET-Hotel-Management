using HotelManagement.Models;

namespace HotelManagement.ViewModels
{
	public class BookingViewModel
	{
		public Customer? Customer { get; set; }
		public Booking? Booking { get; set; }
		public BookingDetail? BookingDetail { get; set; }
		public Category? Category { get; set; }
	}
}
