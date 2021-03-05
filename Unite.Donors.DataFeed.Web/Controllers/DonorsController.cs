using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Donors.DataFeed.Domain.Resources;
using Unite.Donors.DataFeed.Domain.Validation;
using Unite.Donors.DataFeed.Web.Services;

namespace Unite.Donors.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class DonorsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<Donor>> _validator;
        private readonly IDataFeedService _dataFeedService;
        private readonly ILogger _logger; 

        public DonorsController(
            IValidationService validationService,
            IValidator<IEnumerable<Donor>> validator,
            IDataFeedService dataFeedService,
            ILogger<DonorsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataFeedService = dataFeedService;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Donor[] donors)
        {
            if (!_validationService.ValidateParameter(donors, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            _dataFeedService.ProcessDonors(donors);

            return Ok();
        }
    }
}
