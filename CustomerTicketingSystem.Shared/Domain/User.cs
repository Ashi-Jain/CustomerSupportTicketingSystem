using System.Text.Json.Serialization;

namespace CustomerTicketingSystem.Shared.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Role Role { get; set; }

        [JsonIgnore] public ICollection<Ticket> TicketsCreated { get; set; } = new List<Ticket>();
        [JsonIgnore] public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
