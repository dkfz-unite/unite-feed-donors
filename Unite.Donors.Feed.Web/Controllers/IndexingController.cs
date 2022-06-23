using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
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
