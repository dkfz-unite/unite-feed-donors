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

[Route("api/entries")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorsController : Controller
{
    private readonly DonorsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;


    public DonorsController(
        DonorsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] DonorModel[] models)
    {
        var submissionId = _submissionService.AddDonorsSubmission(models);

        long taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DON, submissionId, TaskStatusType.Preparing);

        return Ok(taskId.ToString());
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var task = _submissionTaskService.GetTask(long.Parse(id));
        var submissionDocument = _submissionService.FindDonorsSubmission(task.Target).ToJson();
        return Ok(submissionDocument);
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorTsvModelsBinder))]DonorModel[] models)
    {
        return Post(models);
    }
}
