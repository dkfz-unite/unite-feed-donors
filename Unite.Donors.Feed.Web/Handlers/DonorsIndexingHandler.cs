using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Indices.Services;
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


    public void Prepare()
    {
        _indexingService.UpdateIndex().GetAwaiter().GetResult();
    }

    public void Handle(int bucketSize)
    {
        ProcessDonorIndexingTasks(bucketSize);
    }


    private void ProcessDonorIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        

        _taskProcessingService.Process(IndexingTaskType.Donor, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks() || _taskProcessingService.HasAnnotationTasks())
            {
                return false;
            }

            _logger.LogInformation("Indexing {number} donors", tasks.Length);

            stopwatch.Restart();

            var grouped = tasks.DistinctBy(task => task.Target);

            var indices = grouped.Select(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.AddRange(indices);

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} donors completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
