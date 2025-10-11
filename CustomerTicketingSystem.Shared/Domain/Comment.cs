using System.Text.Json.Serialization;

namespace CustomerTicketingSystem.Shared.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TicketId { get; set; }

        [JsonIgnore]
        public Ticket Ticket { get; set; } = default!;

        public int AuthorId { get; set; }

        public Role AuthorRole { get; set; }
    }
}
