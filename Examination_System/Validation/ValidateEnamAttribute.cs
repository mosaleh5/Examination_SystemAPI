/*using System.ComponentModel.DataAnnotations;

namespace Examination_System.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateEnamAttribute() : ValidationAttribute
    {
        override public bool IsValid(object? value)
        {
            
            var enumType = value.GetType().GetEnumValues().Length - 1;
            if (value == null) return false;
            var intValue = (int)value;
            return intValue >= 1 && intValue <= enumType;
        }

    }
}
*/