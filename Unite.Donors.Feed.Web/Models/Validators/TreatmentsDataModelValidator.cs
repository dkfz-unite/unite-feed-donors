using FluentValidation;
using Unite.Donors.Feed.Web.Models.Base;
using Unite.Donors.Feed.Web.Models.Base.Validators;

namespace Unite.Donors.Feed.Web.Models.Validators;

public class TreatmentsDataModelValidator : AbstractValidator<TreatmentsDataModel>
{
    private readonly IValidator<TreatmentModel> _treatmentModelValidator = new TreatmentModelValidator();
    
    public TreatmentsDataModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.Data)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleForEach(model => model.Data)
            .SetValidator(_treatmentModelValidator);
    }
}

public class TreatmentsDataModelsValidator : AbstractValidator<TreatmentsDataModel[]>
{
    private readonly IValidator<TreatmentsDataModel> _modelValidator = new TreatmentsDataModelValidator();
    
    public TreatmentsDataModelsValidator()
    {
        RuleFor(models => models)
            .Must(models => models.Any())
            .WithMessage("Should not be empty");

        RuleForEach(models => models)
            .SetValidator(_modelValidator);
    }
}
