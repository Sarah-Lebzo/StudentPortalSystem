using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models
{
    public class CourseTeacher
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CourseId { get; set; }
        public Course Course { get; set; }

        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }
    }

}
