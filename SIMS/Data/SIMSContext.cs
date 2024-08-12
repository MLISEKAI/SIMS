using Microsoft.EntityFrameworkCore;
using SIMS;

namespace SIMS.Data
{
    public class SIMSContext : DbContext
    {
        public SIMSContext(DbContextOptions<SIMSContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Students> Students { get; set; } = null!;
        public DbSet<Teachers> Teachers { get; set; } = null!;
        public DbSet<Courses> Courses { get; set; } = null!;
        public DbSet<AdminSystem> AdminSystem { get; set; } = null!;
        public DbSet<Students_Courses> Students_Courses { get; set; } = null!;
        public DbSet<Teachers_Courses> Teachers_Courses { get; set; } = null!;
        public DbSet<Scores> Scores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for Users
            modelBuilder.Entity<Users>()
                .HasKey(u => u.ID);

            modelBuilder.Entity<Users>()
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Users>()
                .Property(u => u.Pass)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Users>()
                .Property(u => u.UserRole)
                .IsRequired()
                .HasMaxLength(100);

            // Configuration for AdminSystem
            modelBuilder.Entity<AdminSystem>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<AdminSystem>(a => a.Admin_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Students
            modelBuilder.Entity<Students>()
                .HasKey(s => s.Student_ID);

            modelBuilder.Entity<Students>()
                .HasMany(s => s.Students_Courses)
                .WithOne(sc => sc.Student)
                .HasForeignKey(sc => sc.Student_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Students>()
                .HasMany(s => s.Scores)
                .WithOne(sc => sc.Student)
                .HasForeignKey(sc => sc.Student_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Teachers
            modelBuilder.Entity<Teachers>()
                .HasKey(t => t.Teacher_ID);

            modelBuilder.Entity<Teachers>()
                .HasMany(t => t.Teachers_Courses)
                .WithOne(tc => tc.Teacher)
                .HasForeignKey(tc => tc.Teacher_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teachers>()
                .HasMany(t => t.Scores)
                .WithOne(s => s.Teacher)
                .HasForeignKey(s => s.Teacher_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Courses
            modelBuilder.Entity<Courses>()
                .HasKey(c => c.Course_ID);

            modelBuilder.Entity<Courses>()
                .HasMany(c => c.Students_Courses)
                .WithOne(sc => sc.Course)
                .HasForeignKey(sc => sc.Course_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Courses>()
                .HasMany(c => c.Teachers_Courses)
                .WithOne(tc => tc.Course)
                .HasForeignKey(tc => tc.Course_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Courses>()
                .HasMany(c => c.Scores)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.Course_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Students_Courses
            modelBuilder.Entity<Students_Courses>()
                .HasKey(sc => new { sc.Student_ID, sc.Course_ID });

            // Configuration for Teachers_Courses
            modelBuilder.Entity<Teachers_Courses>()
                .HasKey(tc => new { tc.Teacher_ID, tc.Course_ID });

            // Configuration for Scores
            modelBuilder.Entity<Scores>()
                .HasKey(s => s.Score_ID);

            modelBuilder.Entity<Scores>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Scores)
                .HasForeignKey(s => s.Student_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Scores>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Scores)
                .HasForeignKey(s => s.Teacher_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Scores>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Scores)
                .HasForeignKey(s => s.Course_ID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
