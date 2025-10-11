using static BCrypt.Net.BCrypt;
using CustomerTicketingSystem.Shared.Data;
using CustomerTicketingSystem.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(AuthDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        // ==============================
        // CUSTOMER REGISTER
        // ==============================
        [HttpPost("customer/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterBody body)
        {
            var email = body.Email.Trim().ToLower();
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return Conflict("Email already used.");

            var user = new User
            {
                FullName = body.FullName,
                Email = email,
                Password = HashPassword(body.Password),
                Role = Role.Customer
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Registration successful. Please login to continue." });
        }

        // ==============================
        // STAFF REGISTER
        // ==============================
        [HttpPost("staff/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterBody body)
        {
            var email = body.Email.Trim().ToLower();
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return Conflict("Email already used.");

            var user = new User
            {
                FullName = body.FullName,
                Email = email,
                Password = HashPassword(body.Password),
                Role = Role.Staff
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Registration successful. Please login to continue." });
        }

        // ==============================
        // CUSTOMER LOGIN
        // ==============================
        [HttpPost("customer/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomer([FromBody] LoginBody body)
        {
            var email = body.Email.Trim().ToLower();
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email && u.Role == Role.Customer);

            if (user is null || !Verify(body.Password, user.Password))
                return Unauthorized("Invalid customer credentials.");

            return Ok(TokenFor(user));
        }

        // ==============================
        // STAFF LOGIN
        // ==============================
        [HttpPost("staff/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginStaff([FromBody] LoginBody body)
        {
            var email = body.Email.Trim().ToLower();
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email && u.Role == Role.Staff);

            if (user is null || !Verify(body.Password, user.Password))
                return Unauthorized("Invalid staff credentials.");

            return Ok(TokenFor(user));
        }

        // ==============================
        // JWT TOKEN GENERATOR
        // ==============================
        private object TokenFor(User user)
        {
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("name", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                user.Id,
                user.FullName,
                user.Email,
                role = user.Role.ToString()
            };
        }

        // ==============================
        // RECORDS
        // ==============================
        public record RegisterBody(string FullName, string Email, string Password);
        public record LoginBody(string Email, string Password);
    }
}
