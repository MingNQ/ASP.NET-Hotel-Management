using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Data
{
	public class HotelDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Staff> Staffs { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<RentForm> RentForms { get; set; }
		public DbSet<Floor> Floors { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Image> Images { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<Rate> Rates { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>().ToTable(nameof(Account));
			modelBuilder.Entity<Staff>().ToTable(nameof(Staff));
			modelBuilder.Entity<Customer>().ToTable(nameof(Customer));
			modelBuilder.Entity<Booking>().ToTable(nameof(Booking));
			modelBuilder.Entity<Invoice>().ToTable(nameof(Invoice));
			modelBuilder.Entity<Payment>().ToTable(nameof(Payment));
			modelBuilder.Entity<RentForm>().ToTable(nameof(RentForm));
			modelBuilder.Entity<Floor>().ToTable(nameof(Floor));
			modelBuilder.Entity<Category>().ToTable(nameof(Category));
			modelBuilder.Entity<Image>().ToTable(nameof(Image));
			modelBuilder.Entity<Service>().ToTable(nameof(Service));
			modelBuilder.Entity<Rate>().ToTable(nameof(Rate));
		}
	}
}
