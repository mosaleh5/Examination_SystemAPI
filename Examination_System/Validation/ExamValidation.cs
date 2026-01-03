using System.ComponentModel.DataAnnotations;

namespace Examination_System.Validation
{
    public class ExamValidation
    {
        /// <summary>
        /// Custom validation attribute to ensure exam date is in the future
        /// </summary>
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

/*
        [AttributeUsage(AttributeTargets.Property)]
        public class PassingScoreAttribute : ValidationAttribute
        {
            private readonly string _fullmarkPropertyName;
            public PassingScoreAttribute(string fullmarkPropertyName)
            {
                _fullmarkPropertyName = fullmarkPropertyName;
            }
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var fullmarkProperty = validationContext.ObjectType.GetProperty(_fullmarkPropertyName);
                if (fullmarkProperty == null)
                {
                    return new ValidationResult($"Unknown property: {_fullmarkPropertyName}");
                }
                var fullmarkValue = fullmarkProperty.GetValue(validationContext.ObjectInstance);
                if (value is int passingScore && fullmarkValue is int fullmark)
                {
                    if (passingScore < 0 || passingScore > fullmark)
                    {
                        return new ValidationResult($"Passing score must be between 0 and {fullmark}.");
                    }
                }
                return ValidationResult.Success;
            }
        }*/
    }
}
