using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class User : BaseModel
    {

        public string UserName { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]

        public string Phone { get; set; }
        public Instructor Instructor { get; set; }
        public Student Student { get; set; }

    }
}
