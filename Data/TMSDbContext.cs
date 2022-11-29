using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Data
{
    public class TMSDbContext : IdentityDbContext
    {
        public DbSet<BooksDB> DbBooks { get; set; }

        public TMSDbContext(DbContextOptions<TMSDbContext> options): base(options)
        {

        }

        public DbSet<UserDB> DbUsers { get; set; }

        public DbSet<TheShoppingCart> DbCart { get; set; }
       
        public DbSet<UserHistory> DbHistory { get; set; }
    }
}
