
using System.IO;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Extensions;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Services.Donors;
using Unite.Donors.Feed.Web.Services.Donors.Converters;
using Unite.Donors.Feed.Web.Services;
using Unite.Essentials.Tsv;
using Unite.Donors.Feed.Web.Services.Treatments.Converters;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
public class TreatmentsController : Controller
{
    //private readonly DonorDataWriter _dataWriter;
    //private readonly DonorIndexingTasksService _indexingTaskService;
    //private readonly ILogger _logger;

    private readonly TreatmentModelConverter _converter;


    public TreatmentsController(
        //DonorDataWriter dataWriter,
        //DonorIndexingTasksService indexingTaskService,
        //ILogger<TreatmentsController> logger
        )
    {
        //_dataWriter = dataWriter;
        //_indexingTaskService = indexingTaskService;
        //_logger = logger;

        _converter = new TreatmentModelConverter();
    }

    [HttpPost]
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

        TreatmentTsvModel[] dataModels = new List<TreatmentTsvModel>().ToArray();
        try
        {
            dataModels = TsvReader.Read<TreatmentTsvModel>(Request.Body).ToArray();
        }
        catch (Exception exception)
        {
            return Json(exception.Message);
        }

        TreatmentModel[] models = dataModels.Select(model => _converter.Convert(model)).ToArray();

        models.ForEach(model => model.Sanitise());

        if (syncIOFeature != null)
        {
            syncIOFeature.AllowSynchronousIO = false;
        }

        return Json(models);
    }

    [HttpPost]
    [Consumes("text/tab-separated-values")]
    public JsonResult UploadTsv()
    {
        var request = Request;

        return Json(Ok());
    }
}
