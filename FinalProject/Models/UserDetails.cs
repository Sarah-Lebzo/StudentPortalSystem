using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class UserDetails
    {
        [Key]
        public Guid Id { get; set; } // مفتاح أساسي منفصل

        [Required]
        public string UserId { get; set; }  // FK إلى AspNetUsers.Id
    }
}
