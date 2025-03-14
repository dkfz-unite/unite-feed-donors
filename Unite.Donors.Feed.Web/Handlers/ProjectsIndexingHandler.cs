using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Indices.Services;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Indices.Entities.Projects;

namespace Unite.Donors.Feed.Web.Handlers;

public class ProjectsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ProjectIndexCreator _indexCreator;
    private readonly IIndexService<ProjectIndex> _indexingService;
    private readonly ILogger _logger;


    public ProjectsIndexingHandler(
        TasksProcessingService taskProcessingService,
        ProjectIndexCreator indexCreator,
        IIndexService<ProjectIndex> indexingService,
        ILogger<ProjectsIndexingHandler> logger)
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
        await ProcessProjectIndexingTasks(bucketSize);
    }


    private async Task ProcessProjectIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.Project, bucketSize, async (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
                return false;

            stopwatch.Restart();

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<ProjectIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                _indexingService.Delete($"{id}").Wait();

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

            _logger.LogInformation("Indexed {number} projects in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
