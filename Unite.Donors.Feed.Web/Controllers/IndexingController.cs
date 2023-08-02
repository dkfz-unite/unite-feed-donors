using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
[Authorize(Roles = Roles.Admin)]
public class IndexingController : Controller
{
    private readonly DonorIndexingTasksService _indexingTaskService;


    public IndexingController(DonorIndexingTasksService indexingTaskService)
    {
        _indexingTaskService = indexingTaskService;
    }

    [HttpPost]
    public IActionResult Donors()
    {
        _indexingTaskService.CreateTasks();

        return Ok();
    }
}
