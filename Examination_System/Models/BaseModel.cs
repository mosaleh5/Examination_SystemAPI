namespace Examination_System.Models
{
    public class BaseModel
    {
        public int ID { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
