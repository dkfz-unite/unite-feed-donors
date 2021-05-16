using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Donors.DataFeed.Web.Models.Donors;
using Unite.Donors.DataFeed.Web.Models.Donors.Converters;
using Unite.Donors.DataFeed.Web.Models.Extensions;
using Unite.Donors.DataFeed.Web.Models.Validation;
using Unite.Donors.Feed.Donors.Data;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class DonorsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<DonorModel>> _validator;
        private readonly DonorDataWriter _dataWriter;
        private readonly DonorIndexingTaskService _indexingTaskService;
        private readonly ILogger _logger;

        private readonly DonorModelConverter _converter;


        public DonorsController(
            IValidationService validationService,
            IValidator<IEnumerable<DonorModel>> validator,
            DonorDataWriter dataWriter,
            DonorIndexingTaskService indexingTaskService,
            ILogger<DonorsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataWriter = dataWriter;
            _indexingTaskService = indexingTaskService;
            _logger = logger;

            _converter = new DonorModelConverter();
        }


        [HttpPost]
        public IActionResult Post([FromBody] DonorModel[] models)
        {
            if (!_validationService.ValidateParameter(models, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            models.ForEach(model => model.Sanitise());

            var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

            _dataWriter.SaveData(dataModels, out var audit);

            _logger.LogInformation(audit.ToString());

            _indexingTaskService.PopulateTasks(audit.Donors);

            return Ok();
        }
    }
}
