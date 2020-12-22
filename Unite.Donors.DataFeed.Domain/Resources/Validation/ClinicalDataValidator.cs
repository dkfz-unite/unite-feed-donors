using FluentValidation;

namespace Unite.Donors.DataFeed.Domain.Resources.Validation
{
    public class ClinicalDataValidator : AbstractValidator<ClinicalData>
    {
        public ClinicalDataValidator()
        {
            RuleFor(clinicalData => clinicalData.Age)
                .GreaterThanOrEqualTo(0).WithMessage("Should be greater than or equal to 0");

            RuleFor(clinicalData => clinicalData.SurvivalDays)
                .GreaterThanOrEqualTo(0).WithMessage("Should be greater than or equal to 0");

            RuleFor(clinicalData => clinicalData.ProgressionFreeDays)
                .GreaterThanOrEqualTo(0).WithMessage("Should be greater than or equal to 0");

            RuleFor(clinicalData => clinicalData.RelapseFreeDays)
                .GreaterThanOrEqualTo(0).WithMessage("Should be greater than or equal to 0");

            RuleFor(clinicalData => clinicalData.KpsBaseline)
                .InclusiveBetween(0, 100).WithMessage("Should be in range [0, 100]");
        }
    }
}
