using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Services;
using Unite.Essentials.Tsv;

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
    public IActionResult Post([FromBody] DonorModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTaskService.PopulateTasks(audit.Donors);

        return Ok();
    }

    [HttpPost("ValidateTsv")]
    [Consumes("text/tab-separated-values")]
    public JsonResult ValidateTsv()
    {
        //Microsoft.AspNetCore.Http.HttpRequest request = Request;

        // Something for stream to be read
        var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
        if (syncIOFeature != null)
        {
            syncIOFeature.AllowSynchronousIO = true;
        }

        DonorTsvModel[] dataModels = new List<DonorTsvModel>().ToArray();
        try
        {
            dataModels = TsvReader.Read<DonorTsvModel>(Request.Body).ToArray();
        }
        catch (Exception exception)
        {
            return Json(exception.Message);
        }

        DonorModel[] models = dataModels.Select(model => _converter.Convert(model)).ToArray();

        models.ForEach(model => model.Sanitise());

        if (syncIOFeature != null)
        {
            syncIOFeature.AllowSynchronousIO = false;
        }

        return Json(models);
    }

    [HttpPost("ValidateJson")]
    [Consumes("application/json")]
    public JsonResult ValidateJson()
    {
        return Json(Ok());
    }

    [HttpPost("UploadTsv")]
    [Consumes("text/tab-separated-values")]
    public JsonResult UploadTsv()
    {
        var request = Request;

        return Json(Ok());
    }
}
