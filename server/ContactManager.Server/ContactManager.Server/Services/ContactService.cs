using ContactManager.Server.Data;
using ContactManager.Server.Dtos;
using ContactManager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Server.Services
{
    public class ContactService
    {
        private readonly AppDbContext _context;

        public ContactService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contact>> GetUserContacts(string userId)
        {
            return await _context.Contacts
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Contact?> GetUserContact(Guid id, string userId)
        {
            return await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task<Contact> CreateContact(ContactDto contactDto, string userId)
        {
            var contact = new Contact
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                Phone = contactDto.Phone,
                Address = contactDto.Address,
                UserId = userId
            };

            if (await _context.Contacts.AnyAsync(c =>
                c.UserId == userId &&
                (c.Email == contactDto.Email || c.Phone == contactDto.Phone)))
            {
                throw new InvalidOperationException("Contact with same email or phone already exists");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Contact?> UpdateContact(Guid id, ContactDto contactDto, string userId)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null) return null;

            if (await _context.Contacts.AnyAsync(c =>
                c.Id != id &&
                c.UserId == userId &&
                (c.Email == contactDto.Email || c.Phone == contactDto.Phone)))
            {
                throw new InvalidOperationException("Another contact with the same email or phone already exists");
            }

            contact.Name = contactDto.Name;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone;
            contact.Address = contactDto.Address;

            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> DeleteContact(Guid id, string userId)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null) return false;

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}