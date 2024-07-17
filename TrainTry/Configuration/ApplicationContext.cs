using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrainTry.Models;

namespace TrainTry.Configuration
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<MemorableDates> MemorableDates { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}