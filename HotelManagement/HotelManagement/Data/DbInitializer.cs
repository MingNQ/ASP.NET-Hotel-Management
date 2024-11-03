using HotelManagement.Models;
using HotelManagement.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Data
{
	public class DbInitializer
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new HotelDbContext(serviceProvider.
				GetRequiredService<DbContextOptions<HotelDbContext>>()))
			{
				context.Database.EnsureCreated();
				if (context.Bookings.Any())
				{
					return;
				}

				// Account
				var accounts = InitAccount();
				foreach (var account in accounts) context.Accounts.Add(account);
				context.SaveChanges();

				// Staff
				var staffs = InitStaff();
				foreach (var staff in staffs) context.Staffs.Add(staff);
				context.SaveChanges();

				// Customer
				var customers = InitCustomer();
				foreach (var customer in customers) context.Customers.Add(customer);
				context.SaveChanges();

				// Booking
				var bookings = InitBooking();
				foreach (var booking in bookings) context.Bookings.Add(booking);
				context.SaveChanges();

				// Category
				var categories = InitCategory();
				foreach (var category in categories) context.Categories.Add(category);
				context.SaveChanges();

				// Room
				var rooms = InitRoom();
				foreach (var room in rooms) context.Rooms.Add(room);
				context.SaveChanges();

				// Invoice
				var invoices = InitInvoice();
				foreach (var invoice in invoices) context.Invoices.Add(invoice);
				context.SaveChanges();

				// Payment
				var payments = InitPayment();
				foreach (var payment in payments) context.Payments.Add(payment);
				context.SaveChanges();

				// RentForm
				var rentForms = InitRentForm();
				foreach (var rentForm in rentForms) context.RentForms.Add(rentForm);
				context.SaveChanges();

				// Image
				var images = InitImage();
				foreach (var image in images) context.Images.Add(image);
				context.SaveChanges();

				// Service
				var services = InitService();
				foreach (var service in services) context.Services.Add(service);
				context.SaveChanges();

				// RoomService
				var roomServices = InitRoomService();
				foreach (var roomService in roomServices) context.RoomServices.Add(roomService);
				context.SaveChanges();

				// Rate
				var rates = InitRate();
				foreach (var rate in rates) context.Rates.Add(rate);
				context.SaveChanges();

				// BookingDetail
				var bookingDetails = InitBookingDetails();
                foreach (var bookingDetail in bookingDetails)
                {
                    var sql = "INSERT INTO BookingDetail (BookingID, CategoryID, NumberRoom) VALUES ({0}, {1}, {2})";
                    context.Database.ExecuteSqlRaw(sql, bookingDetail.BookingID, bookingDetail.CategoryID, bookingDetail.NumberRoom);
                }
            }
		}

		#region Initial Model

		/// <summary>
		/// Account
		/// </summary>
		/// <returns></returns>
		private static Account[] InitAccount()
		{
			var passwordHasher = new PasswordHasher<Account>();

			var accounts = new Account[]
			{
				new Account { Username = "admin", Password = passwordHasher.HashPassword(null, "123"), Type = AccountType.Admin, Active = true },
				new Account { Username = "admin2", Password = passwordHasher.HashPassword(null, "123"), Type = AccountType.Admin, Active = false },
			};

			return accounts;
		}

		/// <summary>
		/// Staff
		/// </summary>
		/// <returns></returns>
		private static Staff[] InitStaff()
		{
			var staffs = new Staff[]
			{
				new Staff { StaffID = "S0001",	AccountID = 1, FirstName = "Nguyen",	LastName = "Quoc Minh",		Gender = Gender.Male,	Address = "Ha Noi",		Email = "minh221230925@lms.utc.edu.vn",		Phone = "0123456788", Role = Role.Manager },
				new Staff { StaffID = "S0002",	FirstName = "Tran",		LastName = "Duc Hai",		Gender = Gender.Male,	Address = "Thai Binh",	Email = "hai221230825@lms.utc.edu.vn",		Phone = "0123456789", Role = Role.Security },
				new Staff { StaffID = "S0003", 	FirstName = "Vu",		LastName = "Mai Lan",		Gender = Gender.Female, Address = "Ha Noi",		Email = "lanvm@gmail.com",					Phone = "0123456787", Role = Role.Receptionist },
			};

			return staffs;
		}

		/// <summary>
		/// Customer
		/// </summary>
		/// <returns></returns>
		private static Customer[] InitCustomer()
		{
			var customers = new Customer[]
			{
				new Customer { CustomerID = "CUS00001",	FirstName = "Nguyen", LastName = "Duc Trong",	Gender = Gender.Male,	Address = "Bac Ninh",		Email = "trong@gmail.com",		Phone = "0223456780", Membership = Membership.Silver },
				new Customer { CustomerID = "CUS00002",	FirstName = "Pham", LastName = "Van Nam",		Gender = Gender.Male,	Address = "Ninh Binh",		Email = "nam@gmail.com",		Phone = "0223456781", Membership = Membership.Gold },
				new Customer { CustomerID = "CUS00003",	FirstName = "Do", LastName = "Ngoc Linh",		Gender = Gender.Female, Address = "Hoa Binh",		Email = "linh@gmail.com",		Phone = "0223456782", Membership = Membership.Silver },
				new Customer { CustomerID = "CUS00004",	FirstName = "Nguyen", LastName = "Van Trung",	Gender = Gender.Male,	Address = "Thai Nguyen",	Email = "trung@gmail.com",		Phone = "0223456788", Membership = Membership.Diamond },
				new Customer { CustomerID = "CUS00005", FirstName = "Tran", LastName = "Van Binh",		Gender = Gender.Male,	Address = "Ha Noi",			Email = "binh@gmail.com",		Phone = "0223456788", Membership = Membership.Gold },
			};

			return customers;
		}

		/// <summary>
		/// Booking
		/// </summary>
		/// <returns></returns>
		private static Booking[] InitBooking()
		{
			var bookings = new Booking[]
			{
				new Booking { BookingID = "BK00001", CustomerID = "CUS00001", NumberPeople = 2, DateCome = DateTime.Parse("2022-01-09"), DateGo = DateTime.Parse("2022-01-12"), Status = true, Deposit = 5000000 },
				new Booking { BookingID = "BK00002", CustomerID = "CUS00002", NumberPeople = 3, DateCome = DateTime.Parse("2022-03-09"), DateGo = DateTime.Parse("2022-03-12"), Status = true, Deposit = 0 },
				new Booking { BookingID = "BK00003", CustomerID = "CUS00003", NumberPeople = 4, DateCome = DateTime.Parse("2022-04-09"), DateGo = DateTime.Parse("2022-04-12"), Status = true, Deposit = 6000000 },
				new Booking { BookingID = "BK00004", CustomerID = "CUS00004", NumberPeople = 2, DateCome = DateTime.Parse("2022-05-09"), DateGo = DateTime.Parse("2022-05-12"), Status = true, Deposit = 4000000 },
				new Booking { BookingID = "BK00005", CustomerID = "CUS00005", NumberPeople = 5, DateCome = DateTime.Parse("2022-02-09"), DateGo = DateTime.Parse("2022-02-12"), Status = true, Deposit = 2000000 },
			};

			return bookings;
		}

		/// <summary>
		/// Room
		/// </summary>
		/// <returns></returns>
		private static Room[] InitRoom()
		{
			var rooms = new Room[]
			{
				new Room { RoomID = "R101", CategoryID = "Standard01", Status = "Vacant" },
				new Room { RoomID = "R102", CategoryID = "Standard01", Status = "Vacant" },
				new Room { RoomID = "R103", CategoryID = "Standard02", Status = "Occupied" },
				new Room { RoomID = "R104", CategoryID = "Standard02", Status = "Vacant" },

				new Room { RoomID = "R201", CategoryID = "Superior01", Status = "Vacant" },
				new Room { RoomID = "R202", CategoryID = "Superior02", Status = "Vacant" },
				new Room { RoomID = "R203", CategoryID = "Superior02", Status = "Occupied" },
				new Room { RoomID = "R204", CategoryID = "Superior03", Status = "Vacant" },

				new Room { RoomID = "R301", CategoryID = "Deluxe01", Status = "Vacant" },
				new Room { RoomID = "R302", CategoryID = "Deluxe02", Status = "Vacant" },
				new Room { RoomID = "R303", CategoryID = "Deluxe01", Status = "Occupied" },
				new Room { RoomID = "R304", CategoryID = "Deluxe02", Status = "Vacant" },

				new Room { RoomID = "R401", CategoryID = "Suite01", Status = "Vacant" },
				new Room { RoomID = "R402", CategoryID = "Suite02", Status = "Vacant" },
				new Room { RoomID = "R403", CategoryID = "Suite03", Status = "Occupied" },
				new Room { RoomID = "R404", CategoryID = "Suite04", Status = "Vacant" },

				new Room { RoomID = "R501", CategoryID = "Standard01", Status = "Vacant" },
				new Room { RoomID = "R502", CategoryID = "Standard02", Status = "Vacant" },
			};

			return rooms;
		}

		/// <summary>
		/// Invoice
		/// </summary>
		/// <returns></returns>
		private static Invoice[] InitInvoice()
		{
			var invoices = new Invoice[]
			{
				new Invoice { InvoiceID = "INV00001", BookingID = "BK00001", StaffID = "S0001" ,DateCreate = DateTime.Parse("2022-01-09 00:00:00.000") },
				new Invoice { InvoiceID = "INV00002", BookingID = "BK00002", StaffID = "S0001" ,DateCreate = DateTime.Parse("2022-01-19 00:00:00.000") },
				new Invoice { InvoiceID = "INV00003", BookingID = "BK00003", StaffID = "S0003" ,DateCreate = DateTime.Parse("2022-01-21 00:00:00.000") },
				new Invoice { InvoiceID = "INV00004", BookingID = "BK00004", StaffID = "S0003" ,DateCreate = DateTime.Parse("2022-01-30 00:00:00.000") },
				new Invoice { InvoiceID = "INV00005", BookingID = "BK00005", StaffID = "S0003" ,DateCreate = DateTime.Parse("2022-02-09 00:00:00.000") },
			};

			return invoices;
		}

		/// <summary>
		/// Payment
		/// </summary>
		/// <returns></returns>
		private static Payment[] InitPayment()
		{
			var payments = new Payment[] 
			{
				new Payment { PaymentID = "P0001", InvoiceID = "INV00001", PaymentMethod = "Cash",			DatePayment = DateTime.Parse("2022-01-12 00:00:00.000"), Status = "Done" },
				new Payment { PaymentID = "P0002", InvoiceID = "INV00002", PaymentMethod = "Credit Card",	DatePayment = DateTime.Parse("2022-03-12 00:00:00.000"), Status = "Done" },
				new Payment { PaymentID = "P0003", InvoiceID = "INV00003", PaymentMethod = "Banking",		DatePayment = DateTime.Parse("2022-04-12 00:00:00.000"), Status = "Done" },
				new Payment { PaymentID = "P0004", InvoiceID = "INV00004", PaymentMethod = "Cash",			DatePayment = DateTime.Parse("2022-05-12 00:00:00.000"), Status = "Done" },
				new Payment { PaymentID = "P0005", InvoiceID = "INV00005", PaymentMethod = "Credit Card",	DatePayment = DateTime.Parse("2022-02-12 00:00:00.000"), Status = "Done" },
			};

			return payments;
		}

		/// <summary>
		/// RentForm
		/// </summary>
		/// <returns></returns>
		private static RentForm[] InitRentForm()
		{
			var rentForms = new RentForm[]
			{
				new RentForm { RentFormID = "RF00001", BookingID = "BK00001", RoomID = "R201", StaffID = "S0001", CustomerID = "CUS00001", DateCreate = DateTime.Parse("2022-01-09 00:00:00.000"), DateCheckIn = DateTime.Parse("2022-01-09 00:00:00.000"), DateCheckOut = DateTime.Parse("2022-01-12 00:00:00.000"), Sale = 0.1m },
				new RentForm { RentFormID = "RF00002", BookingID = "BK00002", RoomID = "R302", StaffID = "S0001", CustomerID = "CUS00002", DateCreate = DateTime.Parse("2022-01-19 00:00:00.000"), DateCheckIn = DateTime.Parse("2022-01-19 00:00:00.000"), DateCheckOut = DateTime.Parse("2022-02-12 00:00:00.000"), Sale = 0.1m },
				new RentForm { RentFormID = "RF00003", BookingID = "BK00003", RoomID = "R202", StaffID = "S0003", CustomerID = "CUS00003", DateCreate = DateTime.Parse("2022-01-21 00:00:00.000"), DateCheckIn = DateTime.Parse("2022-01-21 00:00:00.000"), DateCheckOut = DateTime.Parse("2022-03-12 00:00:00.000"), Sale = 0.1m },
				new RentForm { RentFormID = "RF00004", BookingID = "BK00004", RoomID = "R401", StaffID = "S0003", CustomerID = "CUS00004", DateCreate = DateTime.Parse("2022-01-30 00:00:00.000"), DateCheckIn = DateTime.Parse("2022-01-30 00:00:00.000"), DateCheckOut = DateTime.Parse("2022-04-12 00:00:00.000"), Sale = 0.1m },
				new RentForm { RentFormID = "RF00005", BookingID = "BK00005", RoomID = "R103", StaffID = "S0003", CustomerID = "CUS00005", DateCreate = DateTime.Parse("2022-02-09 00:00:00.000"), DateCheckIn = DateTime.Parse("2022-02-09 00:00:00.000"), DateCheckOut = DateTime.Parse("2022-05-12 00:00:00.000"), Sale = 0.1m },
			};

			return rentForms;
		}

		/// <summary>
		/// Category
		/// </summary>
		/// <returns></returns>
		private static Category[] InitCategory()
		{
			var categories = new Category[]
			{
				new Category { CategoryID = "Standard01",	TypeName = "Standard Single",	Capacity = 20.1m,	Price = 2000000 },
				new Category { CategoryID = "Standard02",	TypeName = "Standard Twin",		Capacity = 20.1m,	Price = 3000000 },
				new Category { CategoryID = "Superior01",   TypeName = "Superior Single",   Capacity = 30.5m,   Price = 4000000 },
				new Category { CategoryID = "Superior02",   TypeName = "Superior Twin",     Capacity = 30.5m,   Price = 4500000 },
				new Category { CategoryID = "Superior03",   TypeName = "Superior Triple",   Capacity = 30.5m,   Price = 5500000 },
				new Category { CategoryID = "Deluxe01",		TypeName = "Deluxe Single",		Capacity = 40.1m,	Price = 7000000 },
				new Category { CategoryID = "Deluxe02",		TypeName = "Deluxe Twin",		Capacity = 20.1m,	Price = 8000000 },
				new Category { CategoryID = "Suite01",		TypeName = "Suite Single",		Capacity = 40.1m,	Price = 9000000 },
				new Category { CategoryID = "Suite02",		TypeName = "Suite Twin",		Capacity = 40.1m,	Price = 9500000 },
				new Category { CategoryID = "Suite03",		TypeName = "Suite Triple",		Capacity = 40.1m,	Price = 10000000 },
				new Category { CategoryID = "Suite04",		TypeName = "Suite Queen",		Capacity = 40.1m,	Price = 10500000 },
			};

			return categories;
		}

		/// <summary>
		/// Image
		/// </summary>
		/// <returns></returns>
		private static Image[] InitImage()
		{
			var images = new Image[] 
			{
				new Image { ImageID = "IMG0001", RoomID = "R101", ImageUrl = "/img/Standard_Room1.jpg" },
				new Image { ImageID = "IMG0002", RoomID = "R101", ImageUrl = "/img/Standard_Room2.jpg" },

				new Image { ImageID = "IMG0003", RoomID = "R102", ImageUrl = "/img/Standard_Room3.jpg" },
				new Image { ImageID = "IMG0004", RoomID = "R102", ImageUrl = "/img/Standard_Room4.jpg" },

				new Image { ImageID = "IMG0005", RoomID = "R103", ImageUrl = "/img/Standard_TwinRoom1.jpg" },
				new Image { ImageID = "IMG0006", RoomID = "R103", ImageUrl = "/img/Standard_TwinRoom2.jpg" },

				new Image { ImageID = "IMG0007", RoomID = "R104", ImageUrl = "/img/Standard_TwinRoom3.jpg" },
				new Image { ImageID = "IMG0008", RoomID = "R104", ImageUrl = "/img/Standard_TwinRoom4.jpg" },

				new Image { ImageID = "IMG0009", RoomID = "R201", ImageUrl = "/img/Superior_Room1.jpg" },
				new Image { ImageID = "IMG0010", RoomID = "R201", ImageUrl = "/img/Superior_Room2.jpg" },

				new Image { ImageID = "IMG0011", RoomID = "R202", ImageUrl = "/img/Superior_TwinRoom1.jpg" },
				new Image { ImageID = "IMG0012", RoomID = "R202", ImageUrl = "/img/Superior_TwinRoom2.jpg" },

				new Image { ImageID = "IMG0013", RoomID = "R203", ImageUrl = "/img/Superior_TwinRoom3.jpg" },
				new Image { ImageID = "IMG0014", RoomID = "R203", ImageUrl = "/img/Superior_TwinRoom4.jpg" },

				new Image { ImageID = "IMG0015", RoomID = "R204", ImageUrl = "/img/Superior_TripleRoom1.jpg" },
				new Image { ImageID = "IMG0016", RoomID = "R204", ImageUrl = "/img/Superior_TripleRoom2.jpg" },

				new Image { ImageID = "IMG0017", RoomID = "R301", ImageUrl = "/img/Deluxe_Room1.jpg" },
				new Image { ImageID = "IMG0018", RoomID = "R301", ImageUrl = "/img/Deluxe_Room2.jpg" },

				new Image { ImageID = "IMG0019", RoomID = "R302", ImageUrl = "/img/Deluxe_TwinRoom1.jpg" },
				new Image { ImageID = "IMG0020", RoomID = "R302", ImageUrl = "/img/Deluxe_TwinRoom2.jpg" },
				
				new Image { ImageID = "IMG0021", RoomID = "R303", ImageUrl = "/img/Deluxe_Room3.jpg" },
				new Image { ImageID = "IMG0022", RoomID = "R303", ImageUrl = "/img/Deluxe_Room4.jpg" },
				
				new Image { ImageID = "IMG0023", RoomID = "R304", ImageUrl = "/img/Deluxe_TwinRoom3.jpg" },
				new Image { ImageID = "IMG0024", RoomID = "R304", ImageUrl = "/img/Deluxe_TwinRoom4.jpg" },

				new Image { ImageID = "IMG0025", RoomID = "R401", ImageUrl = "/img/Suite_Room1.jpg" },
				new Image { ImageID = "IMG0026", RoomID = "R401", ImageUrl = "/img/Suite_Room2.jpg" },
				
				new Image { ImageID = "IMG0027", RoomID = "R402", ImageUrl = "/img/Suite_TwinRoom1.jpg" },
				new Image { ImageID = "IMG0028", RoomID = "R402", ImageUrl = "/img/Suite_TwinRoom2.jpg" },
				
				new Image { ImageID = "IMG0029", RoomID = "R403", ImageUrl = "/img/Suite_TripleRoom1.jpg" },
				new Image { ImageID = "IMG0030", RoomID = "R403", ImageUrl = "/img/Suite_TripleRoom2.jpg" },
				
				new Image { ImageID = "IMG0031", RoomID = "R404", ImageUrl = "/img/Suite_QueenRoom1.jpg" },
				new Image { ImageID = "IMG0032", RoomID = "R404", ImageUrl = "/img/Suite_QueenRoom2.jpg" },

				new Image { ImageID = "IMG0033", RoomID = "R501", ImageUrl = "/img/Standard_Room1.jpg" },
				new Image { ImageID = "IMG0034", RoomID = "R501", ImageUrl = "/img/Standard_Room3.jpg" },

				new Image { ImageID = "IMG0035", RoomID = "R502", ImageUrl = "/img/Standard_Room2.jpg" },
				new Image { ImageID = "IMG0036", RoomID = "R502", ImageUrl = "/img/Standard_Room4.jpg" },
			};

			return images;
		}

		/// <summary>
		/// Service
		/// </summary>
		/// <returns></returns>
		private static Service[] InitService()
		{
			var services = new Service[]
			{
				new Service { ServiceID = "S0001", ServiceName = "Food and Baverage",	Price = 500000 },
				new Service { ServiceID = "S0002", ServiceName = "Wifi",				Price = 0 },
				new Service { ServiceID = "S0003", ServiceName = "Television",			Price = 0 },
				new Service { ServiceID = "S0004", ServiceName = "Air conditioner",		Price = 0 },
				new Service { ServiceID = "S0005", ServiceName = "Laundry",				Price = 100000 },
			};

			return services;
		}

		/// <summary>
		/// Rate
		/// </summary>
		/// <returns></returns>
		private static Rate[] InitRate()
		{
			var rates = new Rate[] 
			{
				new Rate { RateID = "R0001", Username = "Brandon Kelley",	Email = "user1@gmail.com", Point = 5, Message = "After a construction project took longer than expected, my husband, my daughter and I\r\n                            needed a place to stay for a few nights. As a Chicago resident, we know a lot about our\r\n                            city, neighborhood and the types of housing options available and absolutely love our\r\n                            vacation at Sona Hotel.", DateCreate = DateTime.Parse("2022-01-09 00:00:00.000")}, 
				new Rate { RateID = "R0002", Username = "MingNQ",			Email = "user1@gmail.com", Point = 5, Message = "After a construction project took longer than expected, my husband, my daughter and I\r\n                            needed a place to stay for a few nights. As a Chicago resident, we know a lot about our\r\n                            city, neighborhood and the types of housing options available and absolutely love our\r\n                            vacation at Sona Hotel.", DateCreate = DateTime.Parse("2022-01-19 00:00:00.000")}, 
				new Rate { RateID = "R0003", Username = "Batmat",			Email = "user1@gmail.com", Point = 5, Message = "After a construction project took longer than expected, my husband, my daughter and I\r\n                            needed a place to stay for a few nights. As a Chicago resident, we know a lot about our\r\n                            city, neighborhood and the types of housing options available and absolutely love our\r\n                            vacation at Sona Hotel.", DateCreate = DateTime.Parse("2022-01-21 00:00:00.000")}, 
				new Rate { RateID = "R0004", Username = "Superman",			Email = "user1@gmail.com", Point = 5, Message = "After a construction project took longer than expected, my husband, my daughter and I\r\n                            needed a place to stay for a few nights. As a Chicago resident, we know a lot about our\r\n                            city, neighborhood and the types of housing options available and absolutely love our\r\n                            vacation at Sona Hotel.", DateCreate = DateTime.Parse("2022-01-30 00:00:00.000")}, 
			};

			return rates;
		}

		/// <summary>
		/// RoomService 
		/// </summary>
		/// <returns></returns>
		private static RoomService[] InitRoomService()
		{
			var roomService = new RoomService[]
			{
				new RoomService { RoomID = "R101", ServiceID = "S0002" },
				new RoomService { RoomID = "R101", ServiceID = "S0003" },
				new RoomService { RoomID = "R101", ServiceID = "S0004" },

				new RoomService { RoomID = "R102", ServiceID = "S0002" },
				new RoomService { RoomID = "R102", ServiceID = "S0003" },
				new RoomService { RoomID = "R102", ServiceID = "S0004" },

				new RoomService { RoomID = "R103", ServiceID = "S0002" },
				new RoomService { RoomID = "R103", ServiceID = "S0003" },
				new RoomService { RoomID = "R103", ServiceID = "S0004" },

				new RoomService { RoomID = "R104", ServiceID = "S0002" },
				new RoomService { RoomID = "R104", ServiceID = "S0003" },
				new RoomService { RoomID = "R104", ServiceID = "S0004" },

				new RoomService { RoomID = "R201", ServiceID = "S0002" },
				new RoomService { RoomID = "R201", ServiceID = "S0003" },
				new RoomService { RoomID = "R201", ServiceID = "S0004" },

				new RoomService { RoomID = "R202", ServiceID = "S0002" },
				new RoomService { RoomID = "R202", ServiceID = "S0003" },
				new RoomService { RoomID = "R202", ServiceID = "S0004" },

				new RoomService { RoomID = "R203", ServiceID = "S0002" },
				new RoomService { RoomID = "R203", ServiceID = "S0003" },
				new RoomService { RoomID = "R203", ServiceID = "S0004" },

				new RoomService { RoomID = "R204", ServiceID = "S0002" },
				new RoomService { RoomID = "R204", ServiceID = "S0003" },
				new RoomService { RoomID = "R204", ServiceID = "S0004" },

				new RoomService { RoomID = "R301", ServiceID = "S0002" },
				new RoomService { RoomID = "R301", ServiceID = "S0003" },
				new RoomService { RoomID = "R301", ServiceID = "S0004" },

				new RoomService { RoomID = "R302", ServiceID = "S0002" },
				new RoomService { RoomID = "R302", ServiceID = "S0003" },
				new RoomService { RoomID = "R302", ServiceID = "S0004" },

				new RoomService { RoomID = "R303", ServiceID = "S0002" },
				new RoomService { RoomID = "R303", ServiceID = "S0003" },
				new RoomService { RoomID = "R303", ServiceID = "S0004" },

				new RoomService { RoomID = "R304", ServiceID = "S0002" },
				new RoomService { RoomID = "R304", ServiceID = "S0003" },
				new RoomService { RoomID = "R304", ServiceID = "S0004" },

				new RoomService { RoomID = "R401", ServiceID = "S0002" },
				new RoomService { RoomID = "R401", ServiceID = "S0003" },
				new RoomService { RoomID = "R401", ServiceID = "S0004" },

				new RoomService { RoomID = "R402", ServiceID = "S0002" },
				new RoomService { RoomID = "R402", ServiceID = "S0003" },
				new RoomService { RoomID = "R402", ServiceID = "S0004" },

				new RoomService { RoomID = "R403", ServiceID = "S0002" },
				new RoomService { RoomID = "R403", ServiceID = "S0003" },
				new RoomService { RoomID = "R403", ServiceID = "S0004" },

				new RoomService { RoomID = "R404", ServiceID = "S0002" },
				new RoomService { RoomID = "R404", ServiceID = "S0003" },
				new RoomService { RoomID = "R404", ServiceID = "S0004" },

				new RoomService { RoomID = "R501", ServiceID = "S0002" },
				new RoomService { RoomID = "R501", ServiceID = "S0003" },
				new RoomService { RoomID = "R501", ServiceID = "S0004" },

				new RoomService { RoomID = "R502", ServiceID = "S0002" },
				new RoomService { RoomID = "R502", ServiceID = "S0003" },
				new RoomService { RoomID = "R502", ServiceID = "S0004" },
			};

			return roomService;
		}

		public static BookingDetail[] InitBookingDetails()
		{
			var bookingDetails = new BookingDetail[]
			{
				new BookingDetail { BookingID = "BK00001", CategoryID = "Superior01", NumberRoom = 1 },
				new BookingDetail { BookingID = "BK00002", CategoryID = "Deluxe02", NumberRoom = 1 },
				new BookingDetail { BookingID = "BK00003", CategoryID = "Superior01", NumberRoom = 1 },
				new BookingDetail { BookingID = "BK00004", CategoryID = "Suite01", NumberRoom = 1 },
				new BookingDetail { BookingID = "BK00005", CategoryID = "Standard02", NumberRoom = 1 },
			};

			return bookingDetails;
		}

		#endregion
	}
}
