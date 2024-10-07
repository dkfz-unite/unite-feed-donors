using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Handlers.Submission;

public class DonorSubmissionHandler
{
    private readonly DonorsWriter _dataWriter;
    private readonly DonorIndexingTasksService _indexingTaskService;
    private readonly DonorSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;

    private readonly Models.Donors.Converters.DonorModelConverter _modelConverter;

    private readonly ILogger _logger;

    public DonorSubmissionHandler
           (DonorsWriter dataWriter,
           DonorIndexingTasksService indexingTasksService,
           DonorSubmissionService submissionService,
           TasksProcessingService tasksProcessingService,
           ILogger<DonorSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTasksService;
        _submissionService = submissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _modelConverter = new Models.Donors.Converters.DonorModelConverter ();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }

    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DON, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed Donors data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindDonorSubmission(submissionId);
       var convertedData = submittedData.Select(_modelConverter.Convert).ToArray();

        _dataWriter.SaveData(convertedData, out var audit);

        _indexingTaskService.PopulateTasks(audit.Donors);

        _submissionService.DeleteDonorSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}