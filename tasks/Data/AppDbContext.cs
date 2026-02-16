using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using tasks.Models;

namespace tasks.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
