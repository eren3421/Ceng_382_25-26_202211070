using Microsoft.EntityFrameworkCore;
using Ceng382Week5.Models;

namespace Ceng382Week5.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; }
    }
}
