using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Converters;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Unite.Donors.Feed.Web.Configuration.Constants;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class TreatmentsController : Controller
{
    private readonly DonorDataWriter _dataWriter;
    private readonly DonorIndexingTasksService _indexingTaskService;
    private readonly ILogger _logger;

    private readonly TreatmentStandaloneModelConverter _converter;


    public TreatmentsController(
        DonorDataWriter dataWriter,
        DonorIndexingTasksService indexingTaskService,
        ILogger<TreatmentsController> logger
        )
    {
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _logger = logger;

        _converter = new TreatmentStandaloneModelConverter();
    }

    [HttpPost("json")]
    [Consumes("application/json")]
    public IActionResult PostJson([FromBody] TreatmentStandaloneModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelBinder))] TreatmentStandaloneModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }
}
