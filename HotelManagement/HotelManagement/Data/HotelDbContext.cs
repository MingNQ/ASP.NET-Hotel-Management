using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Data
{
	public class HotelDbContext : DbContext
	{
		public HotelDbContext() : base() { }
		public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

		public DbSet<Account> Accounts { get; set; } = null!;
		public DbSet<Staff> Staffs { get; set; } = null!;
		public DbSet<Customer> Customers { get; set; } = null!;
		public DbSet<Booking> Bookings { get; set; } = null!;
		public DbSet<Room> Rooms { get; set; } = null!;
		public DbSet<Invoice> Invoices { get; set; } = null!;
		public DbSet<Payment> Payments { get; set; } = null!;
		public DbSet<RentForm> RentForms { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<Image> Images { get; set; } = null!;
		public DbSet<Service> Services { get; set; } = null!;
		public DbSet<Rate> Rates { get; set; } = null!;
		public DbSet<RoomService> RoomServices { get; set; } = null!;
		public DbSet<BookingDetail> BookingDetails { get; set; } = null!;

		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>().ToTable(nameof(Account));
			modelBuilder.Entity<Staff>().ToTable(nameof(Staff));
			modelBuilder.Entity<Customer>().ToTable(nameof(Customer));
			modelBuilder.Entity<Booking>().ToTable(nameof(Booking));
			modelBuilder.Entity<Invoice>().ToTable(nameof(Invoice));
			modelBuilder.Entity<Payment>().ToTable(nameof(Payment));
			modelBuilder.Entity<RentForm>().ToTable(nameof(RentForm));
			modelBuilder.Entity<Category>().ToTable(nameof(Category));
			modelBuilder.Entity<Image>().ToTable(nameof(Image));
			modelBuilder.Entity<Service>().ToTable(nameof(Service));
			modelBuilder.Entity<Rate>().ToTable(nameof(Rate));
			modelBuilder.Entity<RoomService>().ToTable(nameof(RoomService));
			modelBuilder.Entity<BookingDetail>().ToTable(nameof(BookingDetail)).HasNoKey();
		}
	}
}
