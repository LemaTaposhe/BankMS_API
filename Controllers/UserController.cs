using BankMS_API.Data;
using BankMS_API.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankMS_API.Controllers
{
    [Route("api/User")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserController : ControllerBase
    {
        private readonly BankDbContext _context;

        public UserController(BankDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("GetUsers")]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        [HttpGet, Route("GetUser/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        [HttpPost, Route("InsertUser")]
        public async Task<IActionResult> Post(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict("A user with the same email already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        [HttpDelete, Route("DeleteUser/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"User with ID {id} deleted successfully.");
        }

        [HttpPut, Route("UpdateUser/{id}")]
        public async Task<IActionResult> Put(User user, int id)
        {
            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Role = user.Role;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }
    }
}
