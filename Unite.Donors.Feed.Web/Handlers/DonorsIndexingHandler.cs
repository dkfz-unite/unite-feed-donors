using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;

namespace Unite.Donors.Feed.Web.Handlers;

public class DonorsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly IIndexCreationService<DonorIndex> _indexCreationService;
    private readonly IIndexingService<DonorIndex> _indexingService;
    private readonly ILogger _logger;


    public DonorsIndexingHandler(
        TasksProcessingService taskProcessingService,
        IIndexCreationService<DonorIndex> indexCreationService,
        IIndexingService<DonorIndex> indexingService,
        ILogger<DonorsIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreationService = indexCreationService;
        _indexingService = indexingService;
        _logger = logger;
    }


    public void Prepare()
    {
        _indexingService.UpdateMapping().GetAwaiter().GetResult();
    }

    public void Handle(int bucketSize)
    {
        ProcessDonorIndexingTasks(bucketSize);
    }


    private void ProcessDonorIndexingTasks(int bucketSize)
    {
        _taskProcessingService.Process(TaskType.Indexing, TaskTargetType.Donor, bucketSize, (tasks) =>
        {
            _logger.LogInformation($"Indexing {tasks.Length} donors");

            var stopwatch = Stopwatch.StartNew();

            var indices = tasks.Select(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.IndexMany(indices);

            stopwatch.Stop();

            _logger.LogInformation($"Indexing of {tasks.Length} donors completed in {stopwatch.Elapsed.TotalSeconds}s");

            stopwatch.Reset();
        });
    }
}
