using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Models.Donors.Binders;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
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


    [HttpPost("")]
    [Consumes("application/json", new[] { "application/jsonc" })]
    public IActionResult Post([FromBody] DonorModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorsTsvModelBinder))] DonorModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }
}
