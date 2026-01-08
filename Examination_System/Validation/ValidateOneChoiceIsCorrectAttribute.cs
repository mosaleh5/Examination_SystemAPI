using System.ComponentModel.DataAnnotations;

namespace Examination_System.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateOneChoiceIsCorrectAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var choices = value as IEnumerable<object>;
            if (choices != null)
            {
                int correctCount = 0;
                foreach (var choice in choices)
                {
                    var isCorrectProperty = choice.GetType().GetProperty("IsCorrect");
                    if (isCorrectProperty != null)
                    {
                        var isCorrectValue = isCorrectProperty.GetValue(choice);
                        if (isCorrectValue is bool isCorrect && isCorrect)
                        {
                            correctCount++;
                        }
                    }
                }
                return correctCount == 1;
            }
            return false;
        }
    }
}