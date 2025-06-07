using ContactManager.Server.Data;
using ContactManager.Server.Dtos;
using ContactManager.Server.Models;
using ContactManager.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly byte[] StaticHmacKey = Encoding.UTF8.GetBytes("static-salt");
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
             
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Email already exists.");
            
            using var hmac = new HMACSHA512(StaticHmacKey);
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)))
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Email, user.Name });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) return Unauthorized("Invalid email.");

            using var hmac = new HMACSHA512(StaticHmacKey);
            var computedHash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));
            
            

            if (computedHash != user.PasswordHash)
                return Unauthorized("Invalid password.");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }
    }
}