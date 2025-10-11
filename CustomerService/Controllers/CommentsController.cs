using CustomerTicketingSystem.Shared.Data;
using CustomerTicketingSystem.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId:int}/[controller]")]
    [Authorize(Policy = "CustomerOnly")]
    public class CommentsController : ControllerBase
    {
        private readonly TicketDbContext _db;
        public CommentsController(TicketDbContext db) => _db = db;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST: api/tickets/{ticketId}/comments
        [HttpPost]
        public async Task<ActionResult<Comment>> Add(int ticketId, [FromBody] CommentCreate body)
        {
            var t = await _db.Tickets.FindAsync(ticketId);
            if (t is null) return NotFound();
            if (t.CreatedById != CurrentUserId) return Forbid();

            var c = new Comment { 
                TicketId = ticketId, 
                AuthorId = CurrentUserId, 
                Message = body.Message, 
                AuthorRole = Role.Customer 
            };
            _db.Comments.Add(c);
            t.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(c);
        }

        // GET: api/tickets/{ticketId}/comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> List(int ticketId)
        {
            var t = await _db.Tickets.FindAsync(ticketId);
            if (t is null) return NotFound();
            if (t.CreatedById != CurrentUserId) return Forbid();

            var items = await _db.Comments.Where(c => c.TicketId == ticketId)
                                          .OrderBy(c => c.CreatedAt)
                                          .ToListAsync();
            return Ok(items);
        }

        public record CommentCreate(string Message);
    }
}
