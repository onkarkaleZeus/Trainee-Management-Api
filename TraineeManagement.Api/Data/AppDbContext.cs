using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext( DbContextOptions<AppDbContext> options ) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<Mentor> Mentors { get; set; }

        public DbSet<LearningTask> LearningTasks { get; set; }

        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<SubmissionFile> SubmissionFiles { get; set; }

        public DbSet<ProcessingJob> ProcessingJobs { get; set; }

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