using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Indices.Services;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Indices.Entities.Donors;

namespace Unite.Donors.Feed.Web.Handlers;

public class DonorsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly DonorIndexCreationService _indexCreationService;
    private readonly IIndexService<DonorIndex> _indexingService;
    private readonly ILogger _logger;


    public DonorsIndexingHandler(
        TasksProcessingService taskProcessingService,
        DonorIndexCreationService indexCreationService,
        IIndexService<DonorIndex> indexingService,
        ILogger<DonorsIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreationService = indexCreationService;
        _indexingService = indexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _indexingService.UpdateIndex();
    }

    public async Task Handle(int bucketSize)
    {
        await ProcessDonorIndexingTasks(bucketSize);
    }


    private async Task ProcessDonorIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        

        await _taskProcessingService.Process(IndexingTaskType.Donor, bucketSize, async (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            {
                return false;
            }

            _logger.LogInformation("Indexing {number} donors", tasks.Length);

            stopwatch.Restart();

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<DonorIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await _indexingService.DeleteRange(indicesToDelete);

            if (indicesToCreate.Any())
                await _indexingService.AddRange(indicesToCreate);

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} donors completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
