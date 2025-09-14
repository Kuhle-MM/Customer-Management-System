using Customer_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Management_System.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Address> Address { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.ContactEmail)       
                .IsUnique();    
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.PhoneNumber)       
                .IsUnique();
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.VATNumber)
                .IsUnique();

        }
    }
}
