using CustomerTicketingSystem.Shared.Data;
using CustomerTicketingSystem.Shared.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StaffService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly TicketDbContext _db;
        public TicketsController(TicketDbContext db) => _db = db;

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAll()
        {
            var tickets = await _db.Tickets
                .Include(t => t.Comments)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            return Ok(tickets);
        }

        // GET: api/tickets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetById(int id)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        // PUT: api/tickets/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest body)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            if (!Enum.TryParse<TicketStatus>(body.Status, true, out var parsedStatus))
                return BadRequest("Invalid status value. Allowed: Open, InProgress, Closed");

            ticket.Status = parsedStatus;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(ticket);
        }

        public record UpdateStatusRequest(string Status);

    }
}
