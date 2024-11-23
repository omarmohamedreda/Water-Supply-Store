using Microsoft.EntityFrameworkCore;
using StoreMVC.Models.Entities;

namespace StoreMVC.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options) 
        {
        }

        public DbSet<Product> Products { get; set; }

    }
}
