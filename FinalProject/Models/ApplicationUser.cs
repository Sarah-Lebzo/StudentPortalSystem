using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models
{
    public class ApplicationUser : IdentityUser
    {

        //// أضف هذا الكود لتجنب الخطأ
        //public string FullName { get; set; }

        // علاقة واحد لواحد مع StudentProfile
        public StudentProfile StudentProfile { get; set; }

        // لو المستخدم يدرّس دورات
        public ICollection<CourseTeacher> TeachingCourses { get; set; }
    }
}
