using FluentValidation;

namespace Unite.Donors.Feed.Web.Models.Base.Validators;

public class ClinicalDataModelValidator : AbstractValidator<ClinicalDataModel>
{
    public ClinicalDataModelValidator()
    {
        RuleFor(model => model.Diagnosis)
            .NotEmpty()
            .WithMessage("Shoild not be empty");

        RuleFor(model => model.Diagnosis)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.PrimarySite)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Localization)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Age)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.VitalStatusChangeDate)
            .Empty()
            .When(model => model.VitalStatusChangeDay.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.VitalStatusChangeDay)
           .Empty()
           .When(model => model.VitalStatusChangeDate.HasValue)
           .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.VitalStatusChangeDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.VitalStatusChangeDay.HasValue)
            .WithMessage("Should be greater than or equal to 1");

        RuleFor(model => model.ProgressionStatusChangeDate)
            .Empty()
            .When(model => model.ProgressionStatusChangeDay.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.ProgressionStatusChangeDay)
            .Empty()
            .When(model => model.ProgressionStatusChangeDate.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.ProgressionStatusChangeDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.ProgressionStatusChangeDay.HasValue)
            .WithMessage("Should be greater than or equal to 1");

        RuleFor(model => model.KpsBaseline)
            .InclusiveBetween(0, 100)
            .WithMessage("Should be in range [0, 100]");
    }
}
