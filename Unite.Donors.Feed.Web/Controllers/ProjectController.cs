using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/project")]
[Authorize(Policy = Policies.Data.Writer)]
public class ProjectController : Controller
{
    private readonly ProjectsRemover _dataRemover;
    private readonly ProjectIndexRemover _indexRemover;
    private readonly ProjectIndexingTasksService _tasksService;
    private readonly ILogger _logger;


    public ProjectController(
        ProjectsRemover dataRemover,
        ProjectIndexRemover indexRemover,
        ProjectIndexingTasksService tasksService,
        ILogger<ProjectController> logger)
    {
        _dataRemover = dataRemover;
        _indexRemover = indexRemover;
        _tasksService = tasksService;
        _logger = logger;
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var project = _dataRemover.Find(id);

        if (project != null)
        {
            _tasksService.ChangeStatus(false);
            _tasksService.PopulateTasks([id]);
            _indexRemover.DeleteIndex(id);
            _dataRemover.SaveData(project);
            _tasksService.ChangeStatus(true);

            _logger.LogInformation("Project '{id}' has been deleted", id);

            return Ok();
        }
        else
        {
            _logger.LogWarning("Wrong attempt to delete project '{id}'", id);

            return NotFound();
        }
    }
}
