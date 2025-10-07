using CustomerTicketingSystem.Shared.Data;
using CustomerTicketingSystem.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CustomerOnly")]
    public class TicketsController : ControllerBase
    {
        private readonly TicketDbContext _db;
        public TicketsController(TicketDbContext db) => _db = db;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult<Ticket>> Create([FromBody] TicketCreate body)
        {
            var t = new Ticket { Title = body.Title, Description = body.Description, CreatedById = CurrentUserId };
            _db.Tickets.Add(t);
            await _db.SaveChangesAsync();
            return Ok(t);
        }

        // GET: api/tickets/mine
        //mine is for getting tickets for only customer logged in.
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<Ticket>>> Mine()
        {
            var list = await _db.Tickets
                .Where(t => t.CreatedById == CurrentUserId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/tickets/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ticket>> GetById(int id)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t is null) return NotFound();
            if (t.CreatedById != CurrentUserId) return Forbid();
            return t;
        }

        // PUT: api/tickets/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Ticket>> Update(int id, [FromBody] TicketUpdate body)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t is null) return NotFound();
            if (t.CreatedById != CurrentUserId) return Forbid();

            t.Title = body.Title;
            t.Description = body.Description;
            t.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(t);
        }

        // DELETE: api/tickets/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t is null) return NotFound();
            if (t.CreatedById != CurrentUserId) return Forbid();

            _db.Tickets.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public record TicketCreate(string Title, string Description);
        public record TicketUpdate(string Title, string Description);
    }
}
