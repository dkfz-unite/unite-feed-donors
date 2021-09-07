using FluentValidation;

namespace Unite.Donors.Feed.Web.Services.Donors.Validators
{
    public class TreatmentModelValidator : AbstractValidator<TreatmentModel>
    {
        public TreatmentModelValidator()
        {
            RuleFor(model => model.Therapy)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleFor(model => model.Therapy)
                .MaximumLength(100)
                .WithMessage("Maximum length is 100");
        }
    }
}
