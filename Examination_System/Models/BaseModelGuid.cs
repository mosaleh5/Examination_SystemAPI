namespace Examination_System.Models
{

    public class BaseModelGuid
    {
        public BaseModelGuid()
        {
            Id = Guid.CreateVersion7();
        }

        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    
  
  
}
