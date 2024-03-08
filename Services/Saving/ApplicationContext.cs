using Microsoft.EntityFrameworkCore;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Saving
{
    public class ApplicationContext : DbContext
    {
        public DbSet<HistoryRecord> HistoryRecords { get; set; } = null!;
        public DbSet<Settings> Settings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
        }
    }
}
