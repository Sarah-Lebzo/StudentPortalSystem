using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class LectureSchedule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CourseId { get; set; }
        public Course? Course { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

    }

}
