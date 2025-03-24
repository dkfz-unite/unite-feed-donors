using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly ProjectIndexingTasksService _projectTasksService;
    private readonly DonorIndexingTasksService _donorTasksService;

    public IndexingController(
        ProjectIndexingTasksService projectTasksService,
        DonorIndexingTasksService donorTasksService)
    {
        _projectTasksService = projectTasksService;
        _donorTasksService = donorTasksService;
    }

    [HttpPost("projects")]
    public IActionResult PostProjects()
    {
        _projectTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("donors")]
    public IActionResult Post()
    {
        _donorTasksService.CreateTasks();

        return Ok();
    }
}
