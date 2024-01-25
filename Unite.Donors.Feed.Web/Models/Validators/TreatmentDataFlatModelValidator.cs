using FluentValidation;
using Unite.Donors.Feed.Web.Models.Base;
using Unite.Donors.Feed.Web.Models.Base.Validators;

namespace Unite.Donors.Feed.Web.Models.Validators;

public class TreatmentDataFlatModelValidator : AbstractValidator<TreatmentDataFlatModel>
{
    private readonly IValidator<TreatmentModel> _treatmentModelValidator = new TreatmentModelValidator();
    
    public TreatmentDataFlatModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model)
            .SetValidator(_treatmentModelValidator);
    }
}

public class TreatmentDataFlatModelsValidator : AbstractValidator<TreatmentDataFlatModel[]>
{
    private readonly IValidator<TreatmentDataFlatModel> _modelValidator = new TreatmentDataFlatModelValidator();

    public TreatmentDataFlatModelsValidator()
    {
        RuleFor(models => models)
            .Must(models => models.Any())
            .WithMessage("Should not be empty");

        RuleForEach(models => models)
            .SetValidator(_modelValidator);
    }
}
