using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Data.Exceptions;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models;
using Unite.Donors.Feed.Web.Models.Binders;
using Unite.Donors.Feed.Web.Models.Converters;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class TreatmentsController : Controller
{
    private readonly TreatmentsDataWriter _dataWriter;
    private readonly DonorIndexingTasksService _indexingTaskService;
    private readonly ILogger _logger;

    private readonly TreatmentsDataModelsConverter _defaultModelsConverter = new();
    private readonly TreatmentDataFlatModelsConverter _flatModelsConverter = new();


    public TreatmentsController(
        TreatmentsDataWriter dataWriter,
        DonorIndexingTasksService indexingTaskService,
        ILogger<TreatmentsController> logger
        )
    {
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }

    [HttpPost("")]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] TreatmentsDataModel[] models)
    {
        var dataModels = _defaultModelsConverter.Convert(models);

        return PostData(dataModels);
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelBinder))]TreatmentDataFlatModel[] models)
    {
        var dataModels = _flatModelsConverter.Convert(models);

        return PostData(dataModels);
    }


    private IActionResult PostData(Data.Models.DonorModel[] models)
    {
        try
        {
            _dataWriter.SaveData(models, out var audit);

            _logger.LogInformation("{audit}", audit.ToString());

            _indexingTaskService.PopulateTasks(audit.Donors);

            return Ok();
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning("{error}", exception.Message);

            return BadRequest(exception.Message);
        }
    }
}
