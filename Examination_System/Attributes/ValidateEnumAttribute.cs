using System.ComponentModel.DataAnnotations;

namespace Examination_System.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Handle null case first
            if (value == null) 
                return false;

            // Get the type and verify it's an enum
            var valueType = value.GetType();
            if (!valueType.IsEnum)
                return false;

            // Check if the value is defined in the enum
            return Enum.IsDefined(valueType, value);
        }
    }
}