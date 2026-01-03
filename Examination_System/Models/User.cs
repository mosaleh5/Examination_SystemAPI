using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        // Navigation properties
        public Instructor Instructor { get; set; }
        public Student Student { get; set; }
    }
}
