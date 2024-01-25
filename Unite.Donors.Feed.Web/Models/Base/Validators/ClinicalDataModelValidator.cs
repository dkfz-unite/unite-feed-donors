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
            .WithMessage("Either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set, not both");

        RuleFor(model => model.VitalStatusChangeDay)
           .Empty()
           .When(model => model.VitalStatusChangeDate.HasValue)
           .WithMessage("Either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set, not both");

        RuleFor(model => model.ProgressionStatusChangeDate)
            .Empty()
            .When(model => model.ProgressionStatusChangeDay.HasValue)
            .WithMessage("Either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set, not both");

        RuleFor(model => model.ProgressionStatusChangeDay)
            .Empty()
            .When(model => model.ProgressionStatusChangeDate.HasValue)
            .WithMessage("Either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set, not both");

        RuleFor(model => model.KpsBaseline)
            .InclusiveBetween(0, 100)
            .WithMessage("Should be in range [0, 100]");
    }
}
