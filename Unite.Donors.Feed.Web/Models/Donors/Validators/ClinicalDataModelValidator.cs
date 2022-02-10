using FluentValidation;

namespace Unite.Donors.Feed.Web.Services.Donors.Validators
{
    public class ClinicalDataModelValidator : AbstractValidator<ClinicalDataModel>
    {
        public ClinicalDataModelValidator()
        {
            RuleFor(model => model)
                .Must(HaveAnythingSet)
                .WithMessage("At least one field has to be set");

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

            RuleFor(model => model.KpsBaseline)
                .InclusiveBetween(0, 100)
                .WithMessage("Should be in range [0, 100]");
        }


        private bool HaveAnythingSet(ClinicalDataModel model)
        {
            return model.Gender != null
                || model.Age != null
                || !string.IsNullOrWhiteSpace(model.Diagnosis)
                || model.DiagnosisDate != null
                || !string.IsNullOrWhiteSpace(model.PrimarySite)
                || !string.IsNullOrWhiteSpace(model.Localization)
                || model.VitalStatus != null
                || model.VitalStatusChangeDate != null
                || model.VitalStatusChangeDay != null
                || model.KpsBaseline != null
                || model.SteroidsBaseline != null;
        }
    }
}
