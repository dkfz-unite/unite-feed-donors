using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Extensions;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Services.Donors;
using Unite.Donors.Feed.Web.Services.Donors.Converters;

namespace Unite.Donors.Feed.Web.Controllers
{
    [Route("api/[controller]")]
    public class DonorsController : Controller
    {
        private readonly DonorDataWriter _dataWriter;
        private readonly DonorIndexingTasksService _indexingTaskService;
        private readonly ILogger _logger;

        private readonly DonorModelConverter _converter;


        public DonorsController(
            DonorDataWriter dataWriter,
            DonorIndexingTasksService indexingTaskService,
            ILogger<DonorsController> logger)
        {
            _dataWriter = dataWriter;
            _indexingTaskService = indexingTaskService;
            _logger = logger;

            _converter = new DonorModelConverter();
        }


        [HttpPost]
        public IActionResult Post([FromBody] DonorModel[] models)
        {
            models.ForEach(model => model.Sanitise());

            var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

            _dataWriter.SaveData(dataModels, out var audit);

            _logger.LogInformation(audit.ToString());

            _indexingTaskService.PopulateTasks(audit.Donors);

            return Ok();
        }
    }
}
