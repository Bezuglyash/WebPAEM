using System;
using Microsoft.EntityFrameworkCore;
using NetMarket.Models;

namespace NetMarket.Entities
{
    public class NetMarketDbContext : DbContext
    {
        public NetMarketDbContext() { }

        public NetMarketDbContext(DbContextOptions<NetMarketDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLazyLoadingProxies();

        public DbSet<People> People { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductInBasket> ProductsInBasket { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<OrderStatus> OrdersStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string employeeRoleName = "employee";
            string userRoleName = "user";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role employeeRole = new Role { Id = 2, Name = employeeRoleName };
            Role userRole = new Role { Id = 3, Name = userRoleName };

            string adminLogin = "MAXon28";
            string adminEmail = "max.ronald9@gmail.com";
            string adminPassword = Encryption.Encryption.GetHash("2812");
            string adminName = "Максим";
            string adminSurname = "Безуглый";
            string adminMiddleName = "Викторович";
            string adminPhoneNumber = "89162185817";

            People adminHuman = new People
            {
                Id = Guid.NewGuid(),
                Login = adminLogin,
                Email = adminEmail,
                Password = adminPassword,
                Name = adminName,
                Surname = adminSurname,
                MiddleName = adminMiddleName,
                PhoneNumber = adminPhoneNumber,
                RoleId = adminRole.Id
            };

            OrderStatus firstStatus = new OrderStatus { Id = 1, Status = "Заказ обрабатывается."};
            OrderStatus secondStatus = new OrderStatus { Id = 2, Status = "Заказ принят. Производится доставка." };
            OrderStatus thirdStatus = new OrderStatus { Id = 3, Status = "Заказ доставлен. Ожидается оплата!" };
            OrderStatus fourthStatus = new OrderStatus { Id = 4, Status = "Оплачено!" };

            modelBuilder.Entity<People>().HasData(new People[] { adminHuman });
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, employeeRole, userRole });
            modelBuilder.Entity<OrderStatus>().HasData(new OrderStatus[] { firstStatus, secondStatus, thirdStatus, fourthStatus });
            base.OnModelCreating(modelBuilder);
        }
    }
}