using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/treatments")]
[Authorize(Policy = Policies.Data.Writer)]
public class TreatmentsController : Controller
{
    private readonly DonorsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;


    public TreatmentsController(
        DonorsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody]TreatmentsModel[] models)
    {
        var submissionId = _submissionService.AddTreatmentsSubmission(models);

        _submissionTaskService.CreateTask(SubmissionTaskType.DON_TRT, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelsBinder))]TreatmentsModel[] models)
    {
        return Post(models);
    }
}
