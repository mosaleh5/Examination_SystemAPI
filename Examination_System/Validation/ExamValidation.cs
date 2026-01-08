using System.ComponentModel.DataAnnotations;

namespace Examination_System.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.UtcNow;
            }
            return true;
        }
    }

 
    
}
