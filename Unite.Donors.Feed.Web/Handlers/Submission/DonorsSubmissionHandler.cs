using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Handlers.Submission;

public class DonorsSubmissionHandler
{
    private readonly DonorsWriter _dataWriter;
    private readonly DonorIndexingTasksService _tasksService;
    private readonly DonorsSubmissionService _submissionService;
    private readonly TasksProcessingService _tasksProcessingService;

    private readonly Models.Donors.Converters.DonorModelConverter _modelConverter;

    private readonly ILogger _logger;

    public DonorsSubmissionHandler(
        DonorsWriter dataWriter,
        DonorIndexingTasksService tasksService,
        DonorsSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<DonorsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _submissionService = submissionService;
        _tasksProcessingService = tasksProcessingService;
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

        _tasksProcessingService.Process(SubmissionTaskType.DON, TaskStatusType.Prepared, 1, (tasks) =>
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
        var submittedData = _submissionService.FindDonorsSubmission(submissionId);
        var convertedData = submittedData.Select(_modelConverter.Convert).ToArray();

        _dataWriter.SaveData(convertedData, out var audit);
        _tasksService.PopulateTasks(audit.Donors);
        _submissionService.DeleteDonorsSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}