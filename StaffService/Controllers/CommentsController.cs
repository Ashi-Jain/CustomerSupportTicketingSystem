using CustomerTicketingSystem.Shared.Data;
using CustomerTicketingSystem.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StaffService.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId:int}/[controller]")]
    [Authorize(Policy = "StaffOnly")]
    public class CommentsController : ControllerBase
    {
        private readonly TicketDbContext _db;
        public CommentsController(TicketDbContext db) => _db = db;

        private int CurrentUserId
        {
            get
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(id))
                    throw new UnauthorizedAccessException("Invalid or missing token for Staff.");
                return int.Parse(id);
            }
        }

        // POST: api/tickets/{ticketId}/comments
        [HttpPost]
        public async Task<ActionResult<Comment>> Add(int ticketId, [FromBody] CommentCreate body)
        {
            var ticket = await _db.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found.");

            var comment = new Comment
            {
                TicketId = ticketId,
                AuthorId = CurrentUserId,
                Message = body.Message,
                AuthorRole = Role.Staff
            };

            _db.Comments.Add(comment);
            ticket.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(comment);
        }

        // GET: api/tickets/{ticketId}/comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> Get(int ticketId)
        {
            var exists = await _db.Tickets.AnyAsync(t => t.Id == ticketId);
            if (!exists) return NotFound("Ticket not found.");

            var comments = await _db.Comments
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(comments);
        }

        public record CommentCreate(string Message);
    }
}
