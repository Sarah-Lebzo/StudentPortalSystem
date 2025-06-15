using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FinalProject.Data
{
    public class FinalAuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public FinalAuthDbContext(DbContextOptions<FinalAuthDbContext> options)
           : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Seed Roles (Student, Admin, Teacher)

            var teacherRoleId = "37cc67e1-41ca-461c-bf34-2b5e62dbae32";
            var adminRoleId = "3cfd9eee-08cb-4da3-9e6f-c3166b50d3b0";
            var studentRoleId = "a0cab2c3-6558-4a1c-be81-dfb39180da3d";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name= "Teacher",
                    NormalizedName = "Teacher",
                    Id = teacherRoleId,
                    ConcurrencyStamp = teacherRoleId
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name = "Student",
                    NormalizedName = "Student",
                    Id = studentRoleId,
                    ConcurrencyStamp = studentRoleId
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            // Seed AdminUser
            var AdminId = "472ba632-6133-44a1-b158-6c10bd7d850d";
            var AdminUser = new ApplicationUser
            {
                UserName = "System Administrator",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com".ToUpper(),
                NormalizedUserName = "admin@gmail.com".ToUpper(),
                Id = AdminId,
                //FullName = "System Administrator"
            };

            AdminUser.PasswordHash = new PasswordHasher<ApplicationUser>()
             .HashPassword(AdminUser, "Admin@123");


            builder.Entity<ApplicationUser>().HasData(AdminUser);


            // Add All roles to AdminUser
            var adminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = teacherRoleId,
                    UserId = AdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = AdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = studentRoleId,
                    UserId = AdminId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);

            // 👇 هذا السطر هو المهم لحل المشكلة!
            //builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        }
    }
}
