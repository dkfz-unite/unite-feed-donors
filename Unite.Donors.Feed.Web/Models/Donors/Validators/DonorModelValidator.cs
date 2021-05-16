using System.Collections.Generic;
using FluentValidation;

namespace Unite.Donors.DataFeed.Web.Models.Donors.Validators
{
    public class DonorModelValidator : AbstractValidator<DonorModel>
    {
        private readonly IValidator<ClinicalDataModel> _clinicalDataValidator;
        private readonly IValidator<TreatmentModel> _treatmentModelValidator;

        public DonorModelValidator()
        {
            _clinicalDataValidator = new ClinicalDataModelValidator();
            _treatmentModelValidator = new TreatmentModelValidator();


            RuleFor(model => model.Id)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleFor(model => model.Id)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");

            RuleForEach(model => model.WorkPackages)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleForEach(model => model.WorkPackages)
                .MaximumLength(100)
                .WithMessage("Maximum length is 100");

            RuleForEach(model => model.Studies)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleForEach(model => model.Studies)
                .MaximumLength(100)
                .WithMessage("Maximum length is 100");


            RuleFor(model => model.ClinicalData)
                .SetValidator(_clinicalDataValidator);

            RuleForEach(model => model.Treatments)
                .SetValidator(_treatmentModelValidator);
        }
    }


    public class DonorModelsValidator : AbstractValidator<IEnumerable<DonorModel>>
    {
        private readonly IValidator<DonorModel> _donorModelValidator;

        public DonorModelsValidator()
        {
            _donorModelValidator = new DonorModelValidator();


            RuleForEach(model => model)
                .SetValidator(_donorModelValidator);
        }
    }
}
