using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models;
using Unite.Donors.Feed.Web.Models.Binders;
using Unite.Donors.Feed.Web.Models.Converters;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorsController : Controller
{
    private readonly DonorsDataWriter _dataWriter;
    private readonly DonorIndexingTasksService _indexingTaskService;
    private readonly ILogger _logger;

    private readonly DonorDataModelsConverter _converter = new();


    public DonorsController(
        DonorsDataWriter dataWriter,
        DonorIndexingTasksService indexingTaskService,
        ILogger<DonorsController> logger)
    {
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }


    [HttpPost("")]
    [Consumes("application/json")]
    public IActionResult Post([FromBody]DonorDataModel[] models)
    {
        var dataModels = _converter.Convert(models);

        return PostData(dataModels);
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorsTsvModelBinder))]DonorDataModel[] models)
    {
        var dataModels = _converter.Convert(models);

        return PostData(dataModels);
    }


    private IActionResult PostData(Data.Models.DonorModel[] models)
    {
        _dataWriter.SaveData(models, out var audit);

        _logger.LogInformation("{audit}", audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }
}
