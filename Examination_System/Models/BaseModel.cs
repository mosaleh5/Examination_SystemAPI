namespace Examination_System.Models
{
    public class BaseModel : IBaseModel<int>
    {
        public int Id { get; set; }  // Changed from ID to Id for consistency
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class BaseModelString : IBaseModel<string>
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
