using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinalProject.Data
{
    public class FinalDbContext : DbContext
    {
        public FinalDbContext(DbContextOptions<FinalDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTeacher> CourseTeachers { get; set; }
        public DbSet<LectureSchedule> LectureSchedules { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ هذا السطر يحل المشكلة:
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            builder.Entity<CourseTeacher>()
                .HasOne(ct => ct.Course)
                .WithMany(c => c.CourseTeachers)
                .HasForeignKey(ct => ct.CourseId);

            builder.Entity<CourseTeacher>()
                .HasOne(ct => ct.Teacher)
                .WithMany()
                .HasForeignKey(ct => ct.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseEnrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // أو .Cascade حسب الحاجة

            builder.Entity<CourseEnrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LectureSchedule>()
                .HasOne(ls => ls.Course)
                .WithMany(c => c.LectureSchedules)
                .HasForeignKey(ls => ls.CourseId);

            builder.Entity<StudentProfile>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .HasPrincipalKey<ApplicationUser>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
