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
    private readonly DonorIndexCreator _indexCreator;
    private readonly IIndexService<DonorIndex> _indexingService;
    private readonly ILogger _logger;


    public DonorsIndexingHandler(
        TasksProcessingService taskProcessingService,
        DonorIndexCreator indexCreator,
        IIndexService<DonorIndex> indexingService,
        ILogger<DonorsIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreator = indexCreator;
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
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
        {
            _logger.LogInformation("Waiting for other tasks to complete before indexing donors.");
            return;
        }
        
        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.Donor, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<DonorIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreator.CreateIndex(id);

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

            _logger.LogInformation("Indexed {number} donors in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
