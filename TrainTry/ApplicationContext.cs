using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TrainTry
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<MemorableDates> MemorableDates { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TrainTry;Username=postgres;Password=9001");
        }
    }
}
