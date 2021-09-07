using FluentValidation;

namespace Unite.Donors.Feed.Web.Services.Validation
{
    public interface IValidationService
    {
        bool ValidateParameter<T>(T parameter, IValidator<T> validator, out string errorMessage);
    }
}
