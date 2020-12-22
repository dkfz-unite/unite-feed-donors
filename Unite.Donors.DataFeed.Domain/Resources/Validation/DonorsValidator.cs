using System.Collections.Generic;
using FluentValidation;

namespace Unite.Donors.DataFeed.Domain.Resources.Validation
{
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
