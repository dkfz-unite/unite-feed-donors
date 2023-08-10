using FluentValidation;

namespace Unite.Donors.Feed.Web.Services.Donors.Validators;

public class TreatmentStandaloneModelValidator : AbstractValidator<TreatmentStandaloneModel>
{
    private readonly TreatmentModelValidator _treatmentModelValidator = new();

    public TreatmentStandaloneModelValidator()
    {
        RuleFor(model => model)
            .SetValidator(_treatmentModelValidator);

        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}

