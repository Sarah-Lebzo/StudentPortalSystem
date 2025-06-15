namespace FinalProject.Models
{
    public class CourseEnrollment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

    }
}
