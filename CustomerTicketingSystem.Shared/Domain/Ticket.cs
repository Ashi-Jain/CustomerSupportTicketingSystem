namespace CustomerTicketingSystem.Shared.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

}
