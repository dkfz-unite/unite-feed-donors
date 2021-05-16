using FluentValidation;

namespace Unite.Donors.DataFeed.Web.Models.Validation
{
    public interface IValidationService
    {
        bool ValidateParameter<T>(T parameter, IValidator<T> validator, out string errorMessage);
    }
}
