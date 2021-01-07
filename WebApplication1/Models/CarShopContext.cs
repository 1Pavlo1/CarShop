using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Models
{
    public class CarShopContext : IdentityDbContext<IdentityUser>
    {
        public CarShopContext(DbContextOptions<CarShopContext> options) :
            base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
