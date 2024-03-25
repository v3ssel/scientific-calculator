using System.IO;
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
            var dir = Directory.CreateDirectory($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/ScientificCalculator");
            optionsBuilder.UseSqlite($"Data Source={dir.FullName}/sc_data.db");
        }
    }
}
