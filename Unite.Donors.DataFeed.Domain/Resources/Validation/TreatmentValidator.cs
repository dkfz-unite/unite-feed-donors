using FluentValidation;

namespace Unite.Donors.DataFeed.Domain.Resources.Validation
{
    public class TreatmentValidator: AbstractValidator<Treatment>
    {
        public TreatmentValidator()
        {
            RuleFor(treatment => treatment.Therapy)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");
        }
    }
}
