using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly DonorIndexingTasksService _tasksService;

    public IndexingController(
        DonorIndexingTasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost]
    public IActionResult Post()
    {
        _tasksService.CreateTasks();

        return Ok();
    }
}
