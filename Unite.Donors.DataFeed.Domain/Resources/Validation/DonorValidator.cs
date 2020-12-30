using System.Collections.Generic;
using FluentValidation;

namespace Unite.Donors.DataFeed.Domain.Resources.Validation
{
    public class DonorValidator : AbstractValidator<Donor>
    {
        private readonly IValidator<ClinicalData> _clinicalDataValidator;
        private readonly IValidator<Treatment> _treatmentValidator;

        public DonorValidator()
        {
            _clinicalDataValidator = new ClinicalDataValidator();
            _treatmentValidator = new TreatmentValidator();

            RuleFor(donor => donor.Pid)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(donor => donor.Origin)
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(donor => donor.PrimarySite)
                .MaximumLength(100).WithMessage("Maximum length is 50");

            RuleFor(donor => donor.Diagnosis)
                .MaximumLength(100).WithMessage("Maximum length is 100");


            RuleFor(donor => donor.ClinicalData)
                .SetValidator(_clinicalDataValidator);

            RuleForEach(donor => donor.Treatments)
                .SetValidator(_treatmentValidator);

            RuleForEach(donor => donor.WorkPackages)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleForEach(donor => donor.Studies)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");
        }
    }

    public class DonorsValidator : AbstractValidator<IEnumerable<Donor>>
    {
        private readonly IValidator<Donor> _donorValidator;

        public DonorsValidator()
        {
            _donorValidator = new DonorValidator();

            RuleForEach(donors => donors).SetValidator(_donorValidator);
        }
    }
}
