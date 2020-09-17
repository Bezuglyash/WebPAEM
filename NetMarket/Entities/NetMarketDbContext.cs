using System;
using Microsoft.EntityFrameworkCore;
using NetMarket.Models;

namespace NetMarket.Entities
{
    public class NetMarketDbContext : DbContext
    {
        public NetMarketDbContext() {}

        public NetMarketDbContext(DbContextOptions<NetMarketDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminLogin = "MAXon28";
            string adminEmail = "max.ronald9@gmail.com";
            string adminPassword = Encryption.Encryption.GetHash("2812");
            string adminName = "Максим";
            string adminSurname = "Безуглый";
            string adminMiddleName = "Викторович";
            string adminPhoneNumber = "89162185817";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };

            User adminUser = new User
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

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}