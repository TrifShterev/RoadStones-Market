using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoadStones_Models;

namespace RoadStones_Data.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<InquiryHeader> InquiryHeader { get; set; }

        public DbSet<InquiryDetails> InquiryDetails { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}