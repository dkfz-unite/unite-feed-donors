using FluentValidation;

namespace Unite.Donors.DataFeed.Web.Models.Donors.Validators
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

            RuleFor(model => model.KpsBaseline)
                .InclusiveBetween(0, 100)
                .WithMessage("Should be in range [0, 100]");
        }


        private bool HaveAnythingSet(ClinicalDataModel model)
        {
            return model.Gender != null
                || model.Age != null
                || !string.IsNullOrWhiteSpace(model.Diagnosis)
                || !string.IsNullOrWhiteSpace(model.PrimarySite)
                || !string.IsNullOrWhiteSpace(model.Localization)
                || model.VitalStatus != null
                || model.VitalStatusChangeDay != null
                || model.KpsBaseline != null
                || model.SteroidsBaseline != null;
        }
    }
}
