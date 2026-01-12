using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            Id = Guid.CreateVersion7();
        }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        
        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
