using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Handlers.Submission;

public class TreatmentsSubmissionHandler
{
    private readonly TreatmentsWriter _dataWriter;
    private readonly DonorIndexingTasksService _tasksService;
    private readonly DonorsSubmissionService _submissionService;
    private readonly TasksProcessingService _tasksProcessingService;

    private readonly Models.Donors.Converters.TreatmentsModelConverter _modelConverter;

    private readonly ILogger _logger;

    public TreatmentsSubmissionHandler(
        TreatmentsWriter dataWriter,
        DonorIndexingTasksService tasksService,
        DonorsSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<TreatmentsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _submissionService = submissionService;
        _tasksProcessingService = tasksProcessingService;
        _logger = logger;

        _modelConverter = new Models.Donors.Converters.TreatmentsModelConverter();

    }

    public void Handle()
    {
        ProcessSubmissionTasks();
    }

    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _tasksProcessingService.Process(SubmissionTaskType.DON_TRT, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed Treatments data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindTreatmentsSubmission(submissionId);
        var convertedData = submittedData.Select(_modelConverter.Convert).ToArray();

        _dataWriter.SaveData(convertedData, out var audit);
        _tasksService.PopulateTasks(audit.Donors);
        _submissionService.DeleteTreatmentsSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
