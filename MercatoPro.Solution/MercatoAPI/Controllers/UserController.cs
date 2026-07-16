using MercatoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MercatoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            var existing =  await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existing != null)
            {
                return BadRequest("User with this email already exists.");
            }
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginRequest request)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if(existing == null )
            {
                return BadRequest("existing user not found");
            }

            return existing;
        }

        [HttpPut("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            // 1. email se user dhoondo (FirstOrDefaultAsync, jaisa Login mein kiya tha)
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. agar null hai, NotFound("Email not found") return karo
            if(existing == null)
            {
                return NotFound("Email not found");
            }

            // 3. user.Password ko naya password se update karo
            existing.Password = request.NewPassword;

            // 4. SaveChangesAsync() call karo (await ke sath)
            await _context.SaveChangesAsync();

            // 5. Ok("Password reset successful") return karo
            return Ok("Password reset successful");
        }

    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}


