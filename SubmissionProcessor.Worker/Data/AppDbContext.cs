using Microsoft.EntityFrameworkCore;
using SubmissionProcessor.Worker.Models;

namespace SubmissionProcessor.Worker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProcessingJob> ProcessingJobs { get; set; }

        public DbSet<SubmissionFile> SubmissionFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<ProcessingJob>( e =>
            {
                e.ToTable("ProcessingJobs");
                e.HasIndex( j => j.MessageId ).IsUnique();
                e.Property( j => j.Status ).HasConversion<String>(); 
            });
        } 
    }
}