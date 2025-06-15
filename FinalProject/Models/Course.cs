using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Course
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

        public ICollection<LectureSchedule> LectureSchedules { get; set; } = new List<LectureSchedule>();

        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    }
}
