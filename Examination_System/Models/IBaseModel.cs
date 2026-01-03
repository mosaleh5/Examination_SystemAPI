namespace Examination_System.Models
{
    public interface IBaseModel<TKey>
    {
        TKey Id { get; set; }
        bool IsDeleted { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
