using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class CourseCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please select at least one teacher.")]
        public List<string> SelectedTeacherIds { get; set; } = new List<string>();

        public List<SelectListItem> AllTeachers { get; set; } = new List<SelectListItem>();
    }
}
