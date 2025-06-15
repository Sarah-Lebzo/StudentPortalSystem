using Microsoft.AspNetCore.Identity;
// Models/StudentProfile.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FinalProject.Models
{
    public class StudentProfile
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string? ProfileImagePath { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        //public byte[] ImageData { get; set; }
        //public string ImageMimeType { get; set; }

        // ✅ اختياري: اسم المستخدم للتعديل عليه من الواجهة
        //[NotMapped]
        //public string Username { get; set; }
    }
}


