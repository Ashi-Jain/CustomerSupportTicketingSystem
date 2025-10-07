using CustomerTicketingSystem.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace CustomerTicketingSystem.Shared.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Ticket → Comments
            b.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId);
        }
    }
}
