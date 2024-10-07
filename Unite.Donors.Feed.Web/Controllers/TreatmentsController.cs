using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Data.Exceptions;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/treatments")]
[Authorize(Policy = Policies.Data.Writer)]
public class TreatmentsController : Controller
{
    private readonly TreatmentsWriter _dataWriter;
    private readonly SubmissionTaskService _submissionTaskService;
    private readonly DonorSubmissionService _donorsSubmissionsService;

    private readonly TreatmentsModelConverter _converter = new();


    public TreatmentsController(
        TreatmentsWriter dataWriter,
        SubmissionTaskService submissionTasksService,
        DonorSubmissionService donorsSubmissionsService
        )
    {
        _dataWriter = dataWriter;
        _submissionTaskService = submissionTasksService;
        _donorsSubmissionsService = donorsSubmissionsService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody]TreatmentsModel[] models)
    {
        var submissionId = _donorsSubmissionsService.AddTreatmentsSubmission(models);
        _submissionTaskService.CreateTask(SubmissionTaskType.DON_TRT, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelsBinder))]TreatmentsModel[] models)
    {
        return Post(models);
    }
}
