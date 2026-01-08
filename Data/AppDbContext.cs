using Microsoft.EntityFrameworkCore;
using Moldovan_Paula_Lab4.Models;

namespace Moldovan_Paula_Lab4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<PredictionHistory> PredictionHistories { get; set; }

        public DbSet<MoviePredictionHistory> MoviePredictionHistory { get; set; }

    }
}
