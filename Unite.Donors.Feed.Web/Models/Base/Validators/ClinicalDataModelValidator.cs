using FluentValidation;

namespace Unite.Donors.Feed.Web.Models.Base.Validators;

public class ClinicalDataModelValidator : AbstractValidator<ClinicalDataModel>
{
    public ClinicalDataModelValidator()
    {
        RuleFor(model => model.Diagnosis)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.PrimarySite)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Localization)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.EnrollmentAge)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.VitalStatusChangeDate)
            .Empty()
            .When(model => model.VitalStatusChangeDay.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model)
            .Must(model => AreOneDayApart(model.VitalStatusChangeDate, model.EnrollmentDate))
            .When(model => model.VitalStatusChangeDate.HasValue && model.EnrollmentDate.HasValue)
            .WithMessage("Should be at least a day greater than 'EnrollmentDate'");

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

        RuleFor(model => model)
            .Must(model => AreOneDayApart(model.ProgressionStatusChangeDate, model.EnrollmentDate))
            .When(model => model.ProgressionStatusChangeDate.HasValue && model.EnrollmentDate.HasValue)
            .WithMessage("Should be at least a day greater than 'EnrollmentDate'");

        RuleFor(model => model.ProgressionStatusChangeDay)
            .Empty()
            .When(model => model.ProgressionStatusChangeDate.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");        

        RuleFor(model => model.ProgressionStatusChangeDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.ProgressionStatusChangeDay.HasValue)
            .WithMessage("Should be greater than or equal to 1");

        RuleFor(model => model.Kps)
            .InclusiveBetween(0, 100)
            .WithMessage("Should be in range [0, 100]");
    }

    private static bool AreOneDayApart(DateOnly? targetDate, DateOnly? anchorDate)
    {
        if (!anchorDate.HasValue || !targetDate.HasValue)
            return true;

        var anchorDateTipe = anchorDate.Value.ToDateTime(TimeOnly.MinValue);
        var targetDateTipe = targetDate.Value.ToDateTime(TimeOnly.MinValue);
        var difference = (targetDateTipe - anchorDateTipe).TotalDays;
        return difference >= 1;
    }
}
