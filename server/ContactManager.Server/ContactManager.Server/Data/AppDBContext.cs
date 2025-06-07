
using ContactManager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}