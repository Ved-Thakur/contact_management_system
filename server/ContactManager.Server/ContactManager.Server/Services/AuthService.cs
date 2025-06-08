using ContactManager.Server.Data;
using ContactManager.Server.Dtos;
using ContactManager.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ContactManager.Server.Services
{
    public class AuthService
    {
        private static readonly byte[] StaticHmacKey = Encoding.UTF8.GetBytes("static-salt");
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Returns User on success, null on failure
        public async Task<User?> RegisterUser(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            using var hmac = new HMACSHA512(StaticHmacKey);
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)))
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Returns User on success, null on failure
        public async Task<User?> ValidateLogin(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return null;

            using var hmac = new HMACSHA512(StaticHmacKey);
            var computedHash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)));

            return computedHash == user.PasswordHash ? user : null;
        }
    }
}
