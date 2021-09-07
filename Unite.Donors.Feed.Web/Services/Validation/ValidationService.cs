using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace Unite.Donors.Feed.Web.Services.Validation
{
    public class ValidationService : IValidationService
    {
        public bool ValidateParameter<T>(T parameter, IValidator<T> validator, out string errorMessage)
        {
            if (parameter == null || parameter.Equals(default(T)))
            {
                errorMessage = "Request data is empty";

                return false;
            }
            else
            {
                var validationResult = validator.Validate(parameter);

                if (!validationResult.IsValid)
                {
                    errorMessage = GetErrorMessage(validationResult.Errors);

                    return false;
                }
                else
                {
                    errorMessage = null;

                    return true;
                }
            }
        }

        private string GetErrorMessage(IEnumerable<ValidationFailure> failures)
        {
            var message = new StringBuilder();

            message.AppendLine("Request data has invalid format");

            foreach (var failure in failures)
            {
                message.AppendLine($"{failure.PropertyName} - {failure.ErrorMessage}");
            }

            return message.ToString();
        }
    }
}
