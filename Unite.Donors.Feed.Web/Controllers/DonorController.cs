using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/donor")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorController : Controller
{
    private readonly DonorsRemover _dataRemover;
    private readonly DonorIndexRemover _indexRemover;
    private readonly DonorIndexingTasksService _tasksService;
    private readonly ILogger _logger;


    public DonorController(
        DonorsRemover dataRemover,
        DonorIndexRemover indexRemover,
        DonorIndexingTasksService tasksService,
        ILogger<DonorController> logger)
    {
        _dataRemover = dataRemover;
        _indexRemover = indexRemover;
        _tasksService = tasksService;
        _logger = logger;
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var donor = _dataRemover.Find(id);

        if (donor != null)
        {
            _tasksService.ChangeStatus(false);
            _tasksService.PopulateTasks([id]);
            _indexRemover.DeleteIndex(id);
            _dataRemover.SaveData(donor);
            _tasksService.ChangeStatus(true);

            _logger.LogInformation("Donor '{id}' has been deleted", id);

            return Ok();
        }
        else
        {
            _logger.LogWarning("Wrong attempt to delete donor '{id}'", id);

            return NotFound();
        }
    }
}
