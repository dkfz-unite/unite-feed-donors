using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Data.Exceptions;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/treatments")]
[Authorize(Policy = Policies.Data.Writer)]
public class TreatmentsController : Controller
{
    private readonly TreatmentsWriter _dataWriter;
    private readonly DonorIndexingTasksService _tasksService;
    private readonly ILogger _logger;

    private readonly TreatmentsModelConverter _converter = new();


    public TreatmentsController(
        TreatmentsWriter dataWriter,
        DonorIndexingTasksService tasksService,
        ILogger<TreatmentsController> logger
        )
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _logger = logger;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody]TreatmentsModel[] models)
    {
        try
        {
            var data = models.Select(_converter.Convert).ToArray();

            _dataWriter.SaveData(data, out var audit);
            _tasksService.PopulateTasks(audit.Donors);
            _logger.LogInformation("{audit}", audit.ToString());

            return Ok();
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning("{error}", exception.Message);

            return NotFound(exception.Message);
        }
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelsBinder))]TreatmentsModel[] models)
    {
        return Post(models);
    }
}
