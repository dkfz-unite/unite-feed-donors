using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Binders;
using Unite.Donors.Feed.Web.Models.Donors.Converters;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/entries")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorsController : Controller
{
    private readonly DonorsWriter _dataWriter;
    private readonly DonorSubmissionService _donorSubmissionsService;
    private readonly SubmissionTaskService _submissionTaskService;
    
    private readonly DonorModelConverter _converter = new();


    public DonorsController(
        DonorsWriter dataWriter,
        SubmissionTaskService submissionTaskService,
        DonorSubmissionService donorsSubmissionsService)
    {
        _dataWriter = dataWriter;
        _donorSubmissionsService = donorsSubmissionsService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] DonorModel[] models)
    {
        var submissionId = _donorSubmissionsService.AddDonorSubmission(models);

        _submissionTaskService.CreateTask(SubmissionTaskType.DON, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorTsvModelsBinder))]DonorModel[] models)
    {
        return Post(models);
    }
}
