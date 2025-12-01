using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;
using Unite.Indices.Context;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly IIndexService<Unite.Indices.Entities.Donors.DonorIndex> _donorIndexService;
    private readonly IIndexService<Unite.Indices.Entities.Projects.ProjectIndex> _projectIndexService;
    private readonly ProjectIndexingTasksService _projectTasksService;
    private readonly DonorIndexingTasksService _donorTasksService;

    public IndexingController(
        IIndexService<Unite.Indices.Entities.Donors.DonorIndex> donorIndexService,
        IIndexService<Unite.Indices.Entities.Projects.ProjectIndex> projectIndexService,
        ProjectIndexingTasksService projectTasksService,
        DonorIndexingTasksService donorTasksService)
    {
        _donorIndexService = donorIndexService;
        _projectIndexService = projectIndexService;
        _projectTasksService = projectTasksService;
        _donorTasksService = donorTasksService;
    }

    [HttpPost("projects")]
    public async Task<IActionResult> PostProjects()
    {
        await DeleteIndex(_projectIndexService.DeleteIndex());
        
        _projectTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("donors")]
    public async Task<IActionResult> Post()
    {
        await DeleteIndex(_donorIndexService.DeleteIndex());

        _donorTasksService.CreateTasks();

        return Ok();
    }

    private static async Task DeleteIndex(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // Ignore errors
        }
    }
}
