using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace A_Little_Source_Of_Hope.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<DeleteAccount> DeletedAccount { get; set; }
        public DbSet<Province> Province { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Orphanage> Orphanage { get; set; }
        public DbSet<Volunteer> Volunteer { get; set; } 
        public DbSet<News> News { get; set; } 
        public DbSet<NewsSubscription> NewsSubscriptions { get; set; } 
        public DbSet<CashDonation> CashDonations { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
