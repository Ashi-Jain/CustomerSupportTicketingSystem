using CustomerTicketingSystem.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace CustomerTicketingSystem.Shared.Data
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            // unique email
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // ignore navigations (they belong to TicketDbContext)
            b.Entity<User>().Ignore(u => u.TicketsCreated);
            b.Entity<User>().Ignore(u => u.Comments);
        }
    }
}
