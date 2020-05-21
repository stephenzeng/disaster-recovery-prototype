using Microsoft.EntityFrameworkCore;
using StephenZeng.Prototypes.DisasterRecovery.Domain;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ProjectionBookmark> ProjectionBookmarks { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
