using FluentValidation;
using Unite.Donors.Feed.Web.Models.Base;
using Unite.Donors.Feed.Web.Models.Base.Validators;

namespace Unite.Donors.Feed.Web.Models.Donors.Validators;

public class TreatmentsModelValidator : AbstractValidator<TreatmentsModel>
{
    private readonly IValidator<TreatmentModel> _treatmentModelValidator = new TreatmentModelValidator();
    
    public TreatmentsModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.Entries)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleForEach(model => model.Entries)
            .SetValidator(_treatmentModelValidator);
    }
}

public class TreatmentsDataModelsValidator : AbstractValidator<TreatmentsModel[]>
{
    private readonly IValidator<TreatmentsModel> _modelValidator = new TreatmentsModelValidator();
    
    public TreatmentsDataModelsValidator()
    {
        RuleFor(models => models)
            .Must(models => models.Any())
            .WithMessage("Should not be empty");

        RuleForEach(models => models)
            .SetValidator(_modelValidator);
    }
}
