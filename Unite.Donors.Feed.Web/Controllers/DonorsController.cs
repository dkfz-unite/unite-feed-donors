using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Constants;
using Unite.Donors.Feed.Web.Models;
using Unite.Donors.Feed.Web.Models.Binders;
using Unite.Donors.Feed.Web.Models.Converters;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;

namespace Unite.Donors.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class DonorsController : Controller
{
    private readonly DonorsDataWriter _dataWriter;
    private readonly DonorsDataRemover _dataRemover;
    private readonly DonorIndexRemovalService _indexRemover;
    private readonly DonorIndexingTasksService _indexingTaskService;
    private readonly ILogger _logger;

    private readonly DonorDataModelsConverter _converter = new();


    public DonorsController(
        DonorsDataWriter dataWriter,
        DonorsDataRemover dataRemover,
        DonorIndexRemovalService indexRemover,
        DonorIndexingTasksService indexingTaskService,
        ILogger<DonorsController> logger)
    {
        _dataWriter = dataWriter;
        _dataRemover = dataRemover;
        _indexRemover = indexRemover;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }


    [HttpPost("")]
    [Consumes("application/json")]
    public IActionResult Post([FromBody]DonorDataModel[] models)
    {
        var dataModels = _converter.Convert(models);

        return PostData(dataModels);
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(DonorsTsvModelBinder))]DonorDataModel[] models)
    {
        var dataModels = _converter.Convert(models);

        return PostData(dataModels);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return DeleteData(id);
    }


    private IActionResult PostData(Data.Models.DonorModel[] models)
    {
        _dataWriter.SaveData(models, out var audit);
        
        _indexingTaskService.PopulateTasks(audit.Donors);

        _logger.LogInformation("{audit}", audit.ToString());

        return Ok();
    }

    private IActionResult DeleteData(int id)
    {
        var donor = _dataRemover.Find(id);

        if (donor != null)
        {
            _indexingTaskService.ChangeStatus(false);
            _indexingTaskService.PopulateTasks([id]);
            _indexRemover.DeleteIndex(id);
            _dataRemover.SaveData(donor);
            _indexingTaskService.ChangeStatus(true);

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
