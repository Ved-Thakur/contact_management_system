using ContactManager.Server.Data;
using System.Security.Claims;
using ContactManager.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Server.Controllers
{
    [Authorize] // All endpoints require authentication
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/contact
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Contacts
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        // GET: api/contact/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // POST: api/contact
        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(ContactDto contactDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var contact = new Contact
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                Phone = contactDto.Phone,
                Address = contactDto.Address,
                UserId = userId!
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }

        // PUT: api/contact/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(Guid id, ContactDto contactDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            contact.Name = contactDto.Name;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone;
            contact.Address = contactDto.Address;

            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/contact/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class ContactDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
