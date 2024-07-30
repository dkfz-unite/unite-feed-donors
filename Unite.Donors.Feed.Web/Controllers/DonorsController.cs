using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/entries")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorsController : Controller
{
    private readonly DonorsWriter _dataWriter;
    private readonly DonorIndexingTasksService _tasksService;
    private readonly ILogger _logger;

    private readonly DonorModelConverter _converter = new();


    public DonorsController(
        DonorsWriter dataWriter,
        DonorIndexingTasksService tasksService,
        ILogger<DonorsController> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _logger = logger;
    }


    [HttpPost("")]
    public IActionResult Post([FromBody]DonorModel[] models)
    {
        var data = models.Select(_converter.Convert).ToArray();

        _dataWriter.SaveData(data, out var audit);
        _tasksService.PopulateTasks(audit.Donors);
        _logger.LogInformation("{audit}", audit.ToString());

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorTsvModelsBinder))]DonorModel[] models)
    {
        return Post(models);
    }
}
