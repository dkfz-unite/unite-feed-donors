using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

        long taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DON_TRT, submissionId, TaskStatusType.Preparing);

        return Ok(taskId.ToString());
    }

     [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var task = _submissionTaskService.GetTask(long.Parse(id));
        var submissionDocument = _submissionService.FindTreatmentsSubmission(task.Target).ToJson();
        return Ok(submissionDocument);
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(TreatmentsTsvModelsBinder))]TreatmentsModel[] models)
    {
        return Post(models);
    }
}
